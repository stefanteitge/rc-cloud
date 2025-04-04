﻿using McMaster.Extensions.CommandLineUtils;


namespace RcCloud.DateScraper.Cli.Commands;

[Command(Name = "rcc-scrape", Description = "Dependency Injection sample project")]
[Subcommand(typeof(AllCommand))]
[Subcommand(typeof(ChallengeCommand))]
[Subcommand(typeof(DmcCommand))]
[Subcommand(typeof(JsonCommand))]
[Subcommand(typeof(KleinserieCommand))]
[Subcommand(typeof(MyrcmCommand))]
[Subcommand(typeof(MyrcmBeneluxCommand))]
[Subcommand(typeof(MyrcmClubsCommand))]
[HelpOption]
internal class RootCommand
{
    public void OnExecute()
    {
    }
}
