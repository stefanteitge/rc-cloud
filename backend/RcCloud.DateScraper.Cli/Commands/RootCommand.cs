using McMaster.Extensions.CommandLineUtils;


namespace RcCloud.DateScraper.Cli.Commands;

[Command(Name = "rcc-scrape", Description = "Dependency Injection sample project")]
[Subcommand(typeof(AllCommand))]
[Subcommand(typeof(BeneluxCommand))]
[Subcommand(typeof(ChallengeCommand))]
[Subcommand(typeof(DmcCommand))]
[Subcommand(typeof(KleinserieCommand))]
[Subcommand(typeof(MyrcmCommand))]
[Subcommand(typeof(UpdateClubsCommand))]
[HelpOption]
internal class RootCommand
{
    public void OnExecute()
    {
    }
}
