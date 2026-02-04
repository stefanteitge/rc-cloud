using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using RcCloud.DateScraper.Domain.Clubs;
using RcCloud.FunctionApi.Clubs.Dto;

namespace RcCloud.FunctionApi.Clubs.Functions;

public class UpdateClubsFunction(IClubFileRepository fileRepo, IClubCopyRepository repo, ILogger<UpdateClubsFunction> logger)
{
    [Function("update-clubs")]
    public async Task<Results<BadRequest, Ok<List<ClubDto>>>> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
    {
        await fileRepo.LoadFromGithub();

        var clubs = fileRepo.GetAll().ToList();
        var storeSuccess = await repo.Store(clubs);

        if (!storeSuccess)
        {
            return TypedResults.BadRequest();
        }

        logger.LogInformation("Updated and stored clubs.");

        var dtos = clubs.Select(ClubDto.FromDomain).ToList();
        return TypedResults.Ok(dtos);
    }
}
