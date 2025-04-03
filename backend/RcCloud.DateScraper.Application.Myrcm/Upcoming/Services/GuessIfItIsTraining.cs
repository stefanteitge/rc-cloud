namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public class GuessIfItIsTraining
{
    public bool IsTraining(string raceTitle)
    {
        return raceTitle.Contains("Training", StringComparison.InvariantCultureIgnoreCase);
    }
}