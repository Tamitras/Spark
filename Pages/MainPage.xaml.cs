using Mopups.Services;
using Newtonsoft.Json.Linq;
using Spark.Class;
using Spark.Pages.Popups;
using Spark.ViewModels;

namespace Spark.Pages
{
    public partial class MainPage : ContentPage
    {

        public MainPageViewModel ViewModel { get; set; }

        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();

            this.ViewModel = vm;

            BindingContext = ViewModel;
        }

        async Task<string> AnalyzeImage(FileResult photo)
        {
            string result = string.Empty;

            this.Indicator.IsVisible = true;

            try
            {
                this.ViewModel.Status = "Analysiere Bild";
                var photoStream = await photo.OpenReadAsync();
                var memoryStream = new MemoryStream();
                photoStream.CopyTo(memoryStream);

                var byteArray = memoryStream.ToArray();

                //var byteArray = Helper.ConvertImageToJpeg(photoStream);

                double fileSizeInKB = Helper.GetFileSizeInKB(byteArray);
                Console.WriteLine($"File size original: {fileSizeInKB} KB");

                var resizedData = Helper.ResizeImageToMaxSize(byteArray, 1024, 768);

                fileSizeInKB = Helper.GetFileSizeInKB(resizedData);
                Console.WriteLine($"File size resized: {fileSizeInKB} KB");

                this.ViewModel.Status = "Extrahiere Text aus Bild";
                var extractedText = await OcrService.RecognizeTextAsync(resizedData);
                Console.WriteLine($"Text: {extractedText}");
                this.ViewModel.CurrentMeterReading = extractedText;  // Aktualisieren der Eigenschaft

                var template = "{'HT':'WERT','NT':'WERT'}";

                this.ViewModel.Status = "Validiere Informationen";
                result = await GPT3.AskGPT3($"Bitte extrahiere die Zählerstände für Hochtarif (HT) und Niedertarif (NT) aus den folgenden Informationen und gib sie im spezifizierten JSON-Format zurück. Die Werte sollten als Ganzzahlen ohne Buchstaben dargestellt werden und folgendes template besitzen: {template} Text: {extractedText}");

                Console.WriteLine($"result: {result}");
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature wird nicht unterstützt
            }
            catch (PermissionException pEx)
            {
                // Berechtigungsfehler
            }
            catch (Exception ex)
            {
                // Anderer Fehler
            }


            this.Indicator.IsVisible = false;
            return result;
        }

        async void OnCameraButtonClickedAsync(Object sender, EventArgs e)
        {
            var photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                var result = await this.AnalyzeImage(photo);

                this.OpenValidatePopup(result);
            }
        }

        async void OnOpenMediaFileClickedAsync(System.Object sender, System.EventArgs e)
        {
            var photo = await MediaPicker.PickPhotoAsync();

            if (photo != null)
            {
                var result = await this.AnalyzeImage(photo);

                this.OpenValidatePopup(result);
            }
        }

        async void OpenValidatePopup(string result)
        {
            try
            {
                var parsedResult = JObject.Parse(result);
                string htValue = parsedResult["HT"].ToString();
                string ntValue = parsedResult["NT"].ToString();

                var popup = new ValidationPopup(htValue, ntValue);

                await MopupService.Instance.PushAsync(popup);

                this.ViewModel.Status = string.Empty;

                // In Ihrer Hauptseite oder wo immer Sie das Popup anzeigen

                popup.UserResponse += (isOk) =>
                {
                    if (isOk)
                    {
                        this.ViewModel.CurrentMeterReadingInterpreted = $"HT: {popup.HtValue} \n NT: {popup.NtValue}";
                        this.ViewModel.CurrentMeterReading = null;

                        this.ViewModel.ChartPageViewModel.SparkInfo
                        .Add(new SparkInfo(Int32.Parse(popup.HtValue), Int32.Parse(popup.NtValue)));
                    }
                    else
                    {
                        this.ViewModel.CurrentMeterReading = "Abgebrochen";
                    }

                    Console.WriteLine($"{this.ViewModel.ChartPageViewModel.SparkInfo.First().ID}. Found!");

                    MopupService.Instance.PopAllAsync();
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler: {ex}");
            }
        }
    }
}
