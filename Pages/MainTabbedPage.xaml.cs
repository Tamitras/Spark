namespace Spark.Pages;

using Spark.Pages.Settings;
using Spark.ViewModels;
using Spark.ViewModels.Settings;

public partial class MainTabbedPage : TabbedPage
{
    public MainPageVM MainPageViewModel { get; set; }
    public ChartPageViewModel ChartPageViewModel { get; set; } = new ChartPageViewModel();
    public SettingVM SettingVM { get; set; } = new SettingVM();

    public MainTabbedPage()
    {
        InitializeComponent();

        // Erstellen der ViewModels
        MainPageViewModel = new MainPageVM(ChartPageViewModel, SettingVM);

        // Erstellen der Seiten mit den ViewModels
        var mainPage = new MainPage(MainPageViewModel);
        var chartPage = new ChartPage(ChartPageViewModel);
        var settingsPage = new SettingsPage(SettingVM);

        // Hinzufügen der Seiten zur TabbedPage
        Children.Add(mainPage);
        Children.Add(chartPage);
        Children.Add(settingsPage);
    }
}
