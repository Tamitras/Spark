using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class OcrService
{
    private const string OcrApiUrl = "https://api.ocr.space/parse/image";
    private const string ApiKey = "K88551093288957";

    public static async Task<string> RecognizeTextAsync(byte[] imageBytes)
    {
        try
        {
            using var client = new HttpClient();
            using var content = new MultipartFormDataContent
            {
                { new ByteArrayContent(imageBytes), "file", "image.png" },
                { new StringContent("2"), "OCREngine" },  // Verwenden von Engine 2
                { new StringContent("true"), "scale" },  // Skalierung aktivieren
                { new StringContent("true"), "isTable" },  // Tabellen-Ansicht aktivieren
            };

            client.DefaultRequestHeaders.Add("apiKey", ApiKey);

            var response = await client.PostAsync(OcrApiUrl, content).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();  // Wirft eine Ausnahme, wenn der HTTP-Statuscode ein Fehler ist

            var responseJson = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var parsedResponse = JObject.Parse(responseJson);
            var isErroredOnProcessing = parsedResponse["IsErroredOnProcessing"].ToObject<bool>();

            if (isErroredOnProcessing)
            {
                var errorMessage = parsedResponse["ErrorMessage"].ToString();
                throw new Exception($"OCR API Error: {errorMessage}");
            }

            var extractedText = parsedResponse["ParsedResults"][0]["ParsedText"].ToString();
            return extractedText;
        }
        catch (HttpRequestException httpEx)
        {
            // HTTP-Anfragefehler (z.B. Netzwerkprobleme, Server nicht erreichbar)
            Console.WriteLine($"HTTP Request Error: {httpEx.Message}");
            return null;
        }
        catch (JsonException jsonEx)
        {
            // JSON-Parsing-Fehler
            Console.WriteLine($"JSON Parsing Error: {jsonEx.Message}");
            return null;
        }
        catch (Exception ex)
        {
            // Andere unerwartete Fehler
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            return null;
        }
    }
}
