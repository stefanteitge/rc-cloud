using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using RcCloud.DateScraper.Application.Tamiya.Services;

namespace RcCloud.DateScraper.Application.Ai.Tests;

public class TamiyaAiTest
{
    public const string ResponseBeFile = "Data/response_be.json";
    
    public const string ResponseNlFile = "Data/response_nl.json";
    
    private string _apiKey;
    
    public TamiyaAiTest()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<TamiyaAiTest>()
            .Build();

        var section = configuration.GetSection("OpenAI");
        _apiKey = section["Secret"];
    }
    [Fact]
    public async Task GetAiResposeNl()
    {
        var sut = new TamiyaNetherlandsService(_apiKey);
        string reponseContent = await sut.Extract();
        
        File.WriteAllText("response_nl.json", reponseContent);
    }
    
    [Fact]
    public async Task GetAiResposeBe()
    {
        var sut = new TamiyaBelgiumService(_apiKey);
        string reponseContent = await sut.Extract();
        
        File.WriteAllText("response_be.json", reponseContent);
    }

    [Fact]
    public void ParseReponseBe()
    {
        var response = File.ReadAllText(ResponseBeFile);
        
        var sut = new TamiyaBelgiumService(_apiKey);

        var dateDocument = sut.ParseReponse(response);

        Assert.NotNull(dateDocument);
        Assert.Equal(10, dateDocument.Dates.Count);
    }
    
    [Fact]
    public void ParseReponseNl()
    {
        var response = File.ReadAllText(ResponseBeFile);
        
        var sut = new TamiyaNetherlandsService(_apiKey);

        var dateDocument = sut.ParseReponse(response);

        Assert.NotNull(dateDocument);
        Assert.Equal(10, dateDocument.Dates.Count);
    }
}
