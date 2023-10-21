
using Spark.ViewModels.Settings;

namespace Spark.Pages.Settings;

public partial class SettingsPage : ContentPage
{

    public SettingVM SettingVM { get; set; }

    public SettingsPage(SettingVM settingVM)
	{
		InitializeComponent();

        this.SettingVM = settingVM;
        this.BindingContext = this.SettingVM;
	}

    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (double.TryParse(DigitsEntry.Text, out double currentMeterReading) && currentMeterReading > 0)
        {
            // Speichern Sie die Ziffernanzahl, z.B. in den Einstellungen
            SettingVM.MeterReading = currentMeterReading;

            // Navigieren Sie zu einer anderen Seite oder zeigen Sie eine Bestätigung an
            await DisplayAlert("Erfolg", $"Der aktuelle Zählerstand wurde auf {currentMeterReading} festgelegt.", "OK");
        }
        else
        {
            await DisplayAlert("Fehler", "Bitte geben Sie einen gültige Zählerstand ein.", "OK");
        }
    }
}
