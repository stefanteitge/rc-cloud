using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;

namespace RcCloud.DateScraper.Application.Ai.Tests;

public class TamiyaNlAiTest
{
    public const string ResponseBeFile = "Data/response_be.html";
    
    public const string ResponseNlFile = "Data/response_nl.html";
    
    private string _apiKey;
    
    public TamiyaNlAiTest()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TamiyaNlAiTest>()
            .Build();

        var section = configuration.GetSection("OpenAI");
        _apiKey = section["Secret"];
    }
    [Fact(Skip = "Experiment")]
    public async Task GetAiResposeNl()
    {
        var openai = new ChatClient(model: "gpt-4.1-nano", apiKey: _apiKey);

        //var content = File.ReadAllText("Data/tamiya_nl.html");
        var client = new HttpClient();
        var file = await client.GetAsync("https://www.tamiyacup.nl/wedstrijd-agenda-2025.html");
        var content = await file.Content.ReadAsStringAsync();
        
        ChatCompletion completion = openai.CompleteChat(
            @"Can you extract the race dates from as json from the following html source. 
            Make the dates direct children of the node dates. 
            The key for the date should be dateEnd.
            The key for the race name should title.
            The key for the city name should be location.
            On the page the is a list of clubs. Maybe you can assign the correct and full club name to the key clubName.
            The source: " + content);

        File.WriteAllText("response_nl.html", completion.Content[0].Text);
    }
    
    [Fact(Skip = "Experiment")]
    public async Task GetAiResposeBe()
    {
        var openai = new ChatClient(model: "gpt-4.1-nano", apiKey: _apiKey);

        //var content = File.ReadAllText("Data/tamiya_nl.html");
        var client = new HttpClient();
        var file = await client.GetAsync("https://sites.google.com/view/tamiyacup/kalender-inschrijven");
        var content = await file.Content.ReadAsStringAsync();
        
        ChatCompletion completion = openai.CompleteChat(
            @"Can you extract the race dates from as json from the following html source. 
            Make the dates direct children of the node dates. 
            The key for the date should be dateEnd.
            The key for the race name should title.
            The key for the city name should be location.
            On the page the is a list of clubs. Maybe you can assign the correct and full club name to the key clubName.
            The source: " + content);

        File.WriteAllText("response_be.html", completion.Content[0].Text);
    }

    [Fact(Skip = "Experiment")]
    public void ParseReponseBe()
    {
        var response = File.ReadAllText(ResponseBeFile);
        var jsonBlocks = ExtractJsonBlocks(response);
        
        Assert.Single(jsonBlocks);

        var json = JsonDocument.Parse(jsonBlocks.First());
        Assert.Equal(10, json.RootElement.GetProperty("dates").EnumerateArray().Count());
    }
    
    [Fact(Skip = "Experiment")]
    public void ParseReponseNl()
    {
        var response = File.ReadAllText(ResponseNlFile);
        var jsonBlocks = ExtractJsonBlocks(response);
        
        Assert.Single(jsonBlocks);

        var json = JsonDocument.Parse(jsonBlocks.First());
        Assert.Equal(9, json.RootElement.GetProperty("dates").EnumerateArray().Count());
    }

    private List<string> ExtractJsonBlocks(string markdown)
    {
        var jsonList = new List<string>();
        var regex = new Regex(@"```json\s*(.*?)\s*```", RegexOptions.Singleline | RegexOptions.IgnoreCase);

        var matches = regex.Matches(markdown);
        foreach (Match match in matches)
        {
            jsonList.Add(match.Groups[1].Value.Trim());
        }

        return jsonList;
    }
}
