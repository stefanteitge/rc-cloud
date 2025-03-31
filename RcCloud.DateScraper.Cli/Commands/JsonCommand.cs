﻿using RcCloud.DateScraper.Rck.Application.Services;
using System.Text.Json;

namespace RcCloud.DateScraper.Cli.Commands;

internal class JsonCommand(
    ChallengeService challenge,
    KleinserieService kleinserie)
{
    public async Task OnExecute()
    {
        var all = (await challenge.Parse()).ToList();

        var kleinserieAll = await kleinserie.Parse();
        all.AddRange(kleinserieAll);

        all.Sort((a, b) => a.Date.CompareTo(b.Date));

        // TODO: this should use a DTO
        var bytes = JsonSerializer.SerializeToUtf8Bytes(all, JsonSerializerOptions.Web);
        File.WriteAllBytes("all.json", bytes);
    }
}
