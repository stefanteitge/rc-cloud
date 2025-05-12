using OpenAI.Chat;

namespace RcCloud.DateScraper.Application.Tamiya.Services;

public class BaseTamiyaService
{
    public async Task<string> Extract(string openApiKey, string baseUrl)
    {
        var openai = new ChatClient(model: "gpt-4.1-nano", apiKey: openApiKey);
        
        var client = new HttpClient();
        var file = await client.GetAsync(baseUrl);
        var content = await file.Content.ReadAsStringAsync();
        
        ChatCompletionOptions options = new()
        {
            ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                jsonSchemaFormatName: "math_reasoning",
                jsonSchema: BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "dates": {
                                "type": "array",
                                "items": {
                                    "type": "object",
                                    "properties": {
                                        "dateEnd": { "type": "string" },
                                        "title": { "type": "string" },
                                        "location": { "type": "string" },
                                        "clubName": { "type": "string" }
                                    },
                                    "required": ["dateEnd", "title", "location", "clubName"],
                                    "additionalProperties": false
                                }
                            }
                        },
                        "required": ["dates"],
                        "additionalProperties": false
                    }
                    """u8.ToArray()),
                jsonSchemaIsStrict: true)
        };
        
        List<ChatMessage> messages =
        [
            new UserChatMessage(@"Can you extract the race dates from from the following HTML source.
            On the page the is a list of clubs. Maybe you can assign the correct and full club name.
            The source: " + content),
        ];
        
        ChatCompletion completion = await openai.CompleteChatAsync(messages, options);

        return completion.Content[0].Text;
    }
}
