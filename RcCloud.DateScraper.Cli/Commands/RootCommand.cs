using McMaster.Extensions.CommandLineUtils;
using RcCloud.DateScraper.Cli.Commands;


namespace RcCloud.DateScraper.Cli.Kleinserie
{
    [Command(Name = "rcc-scrape", Description = "Dependency Injection sample project")]
    [Subcommand(typeof(AllCommand))]
    [Subcommand(typeof(ChallengeCommand))]
    [Subcommand(typeof(JsonCommand))]
    [Subcommand(typeof(KleinserieCommand))]
    [HelpOption]
    internal class RootCommand
    {
        public void OnExecute()
        {
        }
    }
}
