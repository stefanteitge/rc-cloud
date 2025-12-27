using McMaster.Extensions.CommandLineUtils;


namespace RcCloud.DateScraper.Cli.Commands;

[Command(Name = "rcc-scrape", Description = "Dependency Injection sample project")]
[Subcommand(typeof(GermanyCommand))]
[Subcommand(typeof(MyrcmBeneluxCommand))]
[Subcommand(typeof(ChallengeCommand))]
[Subcommand(typeof(DmcCommand))]
[Subcommand(typeof(KleinserieCommand))]
[Subcommand(typeof(MyrcmGermanyCommand))]
[Subcommand(typeof(UpdateClubsCommand))]
[HelpOption]
internal class RootCommand
{
    public int OnExecute(CommandLineApplication app)
    {
        app.ShowHelp();
        return 0;
    }
}
