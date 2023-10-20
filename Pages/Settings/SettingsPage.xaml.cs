
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
        if (int.TryParse(DigitsEntry.Text, out int digits) && digits > 0)
        {
            // Speichern Sie die Ziffernanzahl, z.B. in den Einstellungen
            SettingVM.DigitCount = digits;

            // Navigieren Sie zu einer anderen Seite oder zeigen Sie eine Bestätigung an
            await DisplayAlert("Erfolg", $"Die Anzahl der Ziffern wurde auf {digits} festgelegt.", "OK");
        }
        else
        {
            await DisplayAlert("Fehler", "Bitte geben Sie eine gültige Anzahl von Ziffern ein.", "OK");
        }
    }
}
