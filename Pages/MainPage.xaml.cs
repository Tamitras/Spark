namespace Spark.Pages;

using Spark.ViewModels;

public partial class MainPage : ContentPage
{
    public MainPageVM MainPageVM { get; set; }

    public MainPage(MainPageVM vm)
    {
        InitializeComponent();

        this.MainPageVM = vm;

        BindingContext = MainPageVM;
    }

    async Task StartProcess()
    {
        FileResult photo = null;

        try
        {

            photo = null;
            photo = await MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                PhotoVM photoVM = new PhotoVM(photo, this.MainPageVM!.SettingVM!.MeterReading);

                _ = Task.Run(async () =>
                {
                    await this.MainPageVM.ProcessPhotoVMAsync(photoVM);

                });

                await Task.Delay(800);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"EXception {ex}");
        }

        Console.WriteLine("Jetzt müssten sachen angezeigt werden");
    }

    async void OnCameraButtonClickedAsync(Object sender, EventArgs e)
    {
        await StartProcess();
    }

    async void OnOpenMediaFileClickedAsync(System.Object sender, System.EventArgs e)
    {
        //var photo = await MediaPicker.PickPhotoAsync();

        //if (photo != null)
        //{
        //    var result = await this.AnalyzeImage(photo);

        //    this.OpenValidatePopup(result);
        //}
    }

    void OnHtPickerSelectedIndexChanged(System.Object sender, System.EventArgs e)
    {

    }

    void OnNtPickerSelectedIndexChanged(System.Object sender, System.EventArgs e)
    {

    }
}
