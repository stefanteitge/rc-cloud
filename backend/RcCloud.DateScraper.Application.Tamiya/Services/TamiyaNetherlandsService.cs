using System.Text.Json;
using System.Text.RegularExpressions;
using OpenAI.Chat;

namespace RcCloud.DateScraper.Application.Tamiya.Services;

public class TamiyaNetherlandsService(string openApiKey) : BaseTamiyaService
{
    private const string BaseUrl = "https://www.tamiyacup.nl/wedstrijd-agenda-2025.html";

    private string OpenApiKey { get; } = openApiKey;

    public async Task<string> Extract()
        => await Extract(OpenApiKey, BaseUrl);

    public ResposeDatesSchema? ParseReponse(string response)
        => JsonSerializer.Deserialize<ResposeDatesSchema>(response, JsonSerializerOptions.Web);
}
