using FluentResults;

namespace RcCloud.DateScraper.Cli.Output;

internal class FluentException(string baseMessage, Result result) : Exception
{
    public override string Message
        => baseMessage + " (" + string.Join(", ", result.Errors.Select(e => e.Message)) + ")";
}
