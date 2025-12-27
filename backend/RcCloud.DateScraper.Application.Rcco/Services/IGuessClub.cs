using RcCloud.DateScraper.Domain.Clubs;

namespace RcCloud.DateScraper.Application.Rcco.Services;

public interface IGuessClub
{
    Club Guess(string clubName, string raceCountryCode);
}
