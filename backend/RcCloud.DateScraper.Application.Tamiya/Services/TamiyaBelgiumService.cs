using System.Text.Json;
using System.Text.RegularExpressions;
using OpenAI.Chat;

namespace RcCloud.DateScraper.Application.Tamiya.Services;

public class TamiyaBelgiumService(string openApiKey) : BaseTamiyaService
{
    private const string BaseUrl = "https://sites.google.com/view/tamiyacup/kalender-inschrijven";

    private string OpenApiKey { get; } = openApiKey;

    public async Task<string> Extract()
        => await Extract(OpenApiKey, BaseUrl);

    public ResposeDatesSchema? ParseReponse(string response)
        => JsonSerializer.Deserialize<ResposeDatesSchema>(response, JsonSerializerOptions.Web);
}
