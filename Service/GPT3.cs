using System.Text;
using Newtonsoft.Json.Linq;

public class GPT3
{
    private const string Endpoint = "https://api.openai.com/v1/chat/completions";
    private const string ApiKey = "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"; // Ersetzen Sie dies durch Ihren OpenAI API-Schlüssel

    public static async Task<string> AskGPT3(string prompt)
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");

                var payload = new
                {
                    model = "gpt-3.5-turbo",
                    messages = new[]
                    {
                    new { role = "user", content = prompt }
                },
                    temperature = 0.8
                };

                var content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(Endpoint, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var parsedResponse = JObject.Parse(jsonResponse);
                return parsedResponse["choices"]?[0]?["message"]?["content"]?.ToString().Trim();

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return null;
        }

    }
}
