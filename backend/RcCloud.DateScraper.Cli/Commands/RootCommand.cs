using McMaster.Extensions.CommandLineUtils;
using RcCloud.DateScraper.Application.Myrcm.Services;


namespace RcCloud.DateScraper.Cli.Commands
{
    [Command(Name = "rcc-scrape", Description = "Dependency Injection sample project")]
    [Subcommand(typeof(AllCommand))]
    [Subcommand(typeof(ChallengeCommand))]
    [Subcommand(typeof(DmcCommand))]
    [Subcommand(typeof(JsonCommand))]
    [Subcommand(typeof(KleinserieCommand))]
    [Subcommand(typeof(MyrcmCommand))]
    [Subcommand(typeof(MyrcmClub))]
    [HelpOption]
    internal class RootCommand
    {
        public void OnExecute()
        {
        }
    }
}
