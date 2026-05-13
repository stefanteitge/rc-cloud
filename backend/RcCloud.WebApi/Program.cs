using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using RcCloud.DateScraper.Application.Common;
using RcCloud.DateScraper.Application.Dmc.Calendar.Services;
using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;
using RcCloud.DateScraper.Application.Rcco;
using RcCloud.DateScraper.Application.Rcco.Services;
using RcCloud.DateScraper.Application.Rck.Services;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.DateScraper.Domain.Races;
using RcCloud.DateScraper.Infrastructure;
using RcCloud.DateScraper.Infrastructure.Clubs.Mongo;
using RcCloud.DateScraper.Infrastructure.Races;
using RcCloud.WebApi.Clubs.Dto;
using RcCloud.WebApi.Races.Dto;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true);

builder.Services
    .AddInfrastructure()
    .AddScraping();

var app = builder.Build();

app.MapGet("/api/germany", async (IRaceCompilationRepository compilationRepository) =>
{
    var racesDocument = await compilationRepository.Load("germany", "aggregate");

    if (racesDocument is null)
        return Results.NotFound();

    var hasDmc = racesDocument.Races.Any(r => r.Source == "DMC");

    string? lastDmcUpdate = null;
    if (!hasDmc)
    {
        var dmcDocument = await compilationRepository.Load("germany", "dmc");
        if (dmcDocument is not null)
        {
            racesDocument.Races.AddRange(dmcDocument.Races);
            lastDmcUpdate = dmcDocument.LastUpdate;
        }
    }

    var today = DateOnly.FromDateTime(DateTime.Now);
    var races = racesDocument.Races.Where(r => r.Date >= today).ToList();

    return Results.Ok(RacePageDto.FromRaces(races, RacePageDto.GermanyRegions, racesDocument.LastUpdate, lastDmcUpdate));
});

app.MapGet("/api/benelux", async (IRaceCompilationRepository compilationRepository) =>
{
    var racesDocument = await compilationRepository.Load("benelux", "aggregate");

    if (racesDocument is null)
        return Results.NotFound();

    var today = DateOnly.FromDateTime(DateTime.Now);
    var races = racesDocument.Races.Where(r => r.Date >= today).ToList();

    return Results.Ok(RacePageDto.FromRaces(races, RacePageDto.BeneluxCategories, racesDocument.LastUpdate, null));
});

app.MapMethods("/api/update-germany", ["GET", "POST"], async (
    ScrapeChallengeRaces challenge,
    ScrapeKleinserieRaces kleinserie,
    ScrapeLrpOffroadRaces lrpOffroad,
    ScrapeMyrcmRaces myrcm,
    ScrapeRcco rcco,
    IClubFileRepository clubFileRepository,
    IClubCopyRepository mongoClubRepository,
    IRaceCompilationRepository repo,
    ILogger<Program> logger) =>
{
    var clubs = await mongoClubRepository.GetAll("germany");
    clubFileRepository.Load(clubs);

    var all = new List<RaceMeeting>();

    var lrpOffroadAll = await lrpOffroad.Scrape();
    all.AddRange(lrpOffroadAll);
    await repo.Store(lrpOffroadAll, "germany", "lrp");

    var challengeAll = await challenge.Scrape();
    all.AddRange(challengeAll);
    await repo.Store(challengeAll, "germany", "challenge");

    var kleinserieAll = await kleinserie.Scrape();
    all.AddRange(kleinserieAll);
    await repo.Store(kleinserieAll, "germany", "kleinserie");

    var myrcmAll = await myrcm.Scrape([MyrcmCountryCode.Germany]);
    all.AddRange(myrcmAll);
    await repo.Store(myrcmAll, "germany", "myrcm");

    var rccoAll = await rcco.Scrape();
    all.AddRange(rccoAll);
    await repo.Store(rccoAll, "germany", "rcco");

    all.Sort((a, b) => a.Date.CompareTo(b.Date));
    await repo.Store(all, "germany", "aggregate");

    logger.LogInformation("Found {Count} races from all sources.", all.Count);

    return Results.Ok(RacePageDto.FromRaces(all, RacePageDto.GermanyRegions, DateTimeOffset.Now.ToString(), null));
});

app.MapGet("/api/update-dmc", async (
    ScrapeDmcRaces dmc,
    IClubFileRepository clubFileRepository,
    IClubCopyRepository mongoClubRepository,
    IRaceCompilationRepository repo,
    ILogger<Program> logger) =>
{
    var clubs = await mongoClubRepository.GetAll("germany");
    clubFileRepository.Load(clubs);

    var dmcResult = await dmc.Scrape(DateTime.Now.Year);
    if (dmcResult.IsFailed)
    {
        return Results.BadRequest(dmcResult.Errors.FirstOrDefault()?.Message ?? "Scraping DMC failed.");
    }

    await repo.Store(dmcResult.Value, "germany", "dmc");
    logger.LogInformation("Found {Count} races from DMC.", dmcResult.Value.Count());

    return Results.Ok(RacePageDto.FromRaces(dmcResult.Value, RacePageDto.GermanyRegions, DateTimeOffset.Now.ToString(), DateTimeOffset.Now.ToString()));
});

app.MapMethods("/api/update-benelux", ["GET", "POST"], async (
    ScrapeMyrcmRaces myrcm,
    IClubFileRepository clubFileRepository,
    IClubCopyRepository mongoClubRepository,
    IRaceCompilationRepository repo,
    ILogger<Program> logger) =>
{
    var clubs = await mongoClubRepository.GetAll("germany");
    clubFileRepository.Load(clubs);

    var all = new List<RaceMeeting>();

    var myrcmAll = await myrcm.Scrape([MyrcmCountryCode.Belgium, MyrcmCountryCode.Luxembourg, MyrcmCountryCode.Netherlands]);
    all.AddRange(myrcmAll);

    all.Sort((a, b) => a.Date.CompareTo(b.Date));
    await repo.Store(all, "benelux", "aggregate");

    logger.LogInformation("Found {Count} races in BeNeLux from all sources.", all.Count);

    return Results.Ok(RacePageDto.FromRaces(all, RacePageDto.BeneluxCategories, DateTimeOffset.Now.ToString(), null));
});

app.MapGet("/api/update-clubs", async (
    IClubFileRepository fileRepo,
    IClubCopyRepository repo,
    ILogger<Program> logger) =>
{
    await fileRepo.LoadFromGithub();

    var clubs = fileRepo.GetAll().ToList();
    var storeSuccess = await repo.Store(clubs);

    if (!storeSuccess)
        return Results.BadRequest();

    logger.LogInformation("Updated and stored clubs.");

    return Results.Ok(clubs.Select(ClubDto.FromDomain).ToList());
});

app.MapGet("/api/clubs", async (IClubCopyRepository repository) =>
{
    var clubs = await repository.GetAll("germany");
    return Results.Ok(clubs.Select(ClubDto.FromDomain).ToList());
});

app.Run();
