using RcCloud.DateScraper.Application.Myrcm.Common.Domain;
using System.Text.Json;

namespace RcCloud.DateScraper.Application.Myrcm.Upcoming.Services;

public class DownloadFilter(MyrcmCountryCode[] myrcmCountry)
{
    private readonly MyrcmCountryCode[] countries = myrcmCountry;

    public string GetCookie()
    {
        var options = new JsonSerializerOptions(JsonSerializerOptions.Web)
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        var json = JsonSerializer.Serialize(GetCountryFilters(), options);
        return Base64Encode(json);
    }

    private string Base64Encode(string plain)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(plain);
        return Convert.ToBase64String(bytes);
    }

    private SingleFilter[] GetCountryFilters()
    {
        if (this.countries.Length == 1)
        {
            var first = countries.First();
            var filter = new SingleFilter(
                new FilterComponent("Country", "Country"),
                new FilterComponent("equals", "eq"),
                new FilterComponent($"\"{first.Name}\"", first.Code.ToString()));

            return [filter];
        }

        var names = "(" + string.Join(", ", countries.Select(c => c.Name)) + ")";
        var values = string.Join(",", countries.Select(c => c.Code));
        var multi = new SingleFilter(
            new FilterComponent("Country", "Country"),
            new FilterComponent("any of", "in"),
            new FilterComponent(names, values));

        return [multi];
    }

    public class SingleFilter(FilterComponent field, FilterComponent op, FilterComponent value)
    {
        public FilterComponent Field { get; } = field;

        public FilterComponent Operator { get; } = op;

        public FilterComponent Value { get; } = value;
    }

    public class FilterComponent(string label, string value)
    {
        public string Label { get; } = label;

        public string Value { get; } = value;
    }
}
