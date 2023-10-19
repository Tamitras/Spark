using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;

namespace Spark.Service;

public class GVisionService
{
    private ImageAnnotatorClient _client;

    public GVisionService()
    {

    }

    public static async Task<GoogleCredential> GetGoogleCredentialFromAssetAsync()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("GVision.json");
        var ret = GoogleCredential.FromStream(stream);

        return ret;
    }

    public async Task<string> RecognizeTextAsync(byte[] imageBytes)
    {
        try
        {
            var credential = await GetGoogleCredentialFromAssetAsync();

            var clientBuilder = new ImageAnnotatorClientBuilder
            {
                Credential = credential
            };
            _client = clientBuilder.Build();

            var image = Google.Cloud.Vision.V1.Image.FromBytes(imageBytes);

            var response = await _client.DetectTextAsync(image);

            return string.Join(Environment.NewLine, response.Select(r => r.Description));
        }
        catch (Exception ex)
        {
            // Je nach Ihren Anforderungen können Sie hier einen Logger verwenden oder die Ausnahme weiter nach oben werfen
            throw new InvalidOperationException("Fehler bei der Texterkennung.", ex);
        }
    }
}
