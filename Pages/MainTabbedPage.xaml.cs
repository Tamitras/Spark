namespace Spark.Pages;

using Spark.Pages.List;
using Spark.Pages.Settings;
using Spark.ViewModels;
using Spark.ViewModels.List;
using Spark.ViewModels.Settings;

public partial class MainTabbedPage : TabbedPage
{
    public MainPageVM MainPageViewModel { get; set; }
    public ChartPageViewModel ChartPageViewModel { get; set; } = new ChartPageViewModel();
    public SettingVM SettingVM { get; set; } = new SettingVM();
    public ListPageVM ListPageVM { get; set; } = new ListPageVM();

    public MainTabbedPage()
    {
        InitializeComponent();

        // Erstellen der ViewModels
        MainPageViewModel = new MainPageVM(ChartPageViewModel, SettingVM, ListPageVM);

        // Set reference ob Photos
        try
        {
            ListPageVM.Init(MainPageViewModel.Photos);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Trying to init listpageVM");
        }

        // Erstellen der Seiten mit den ViewModels
        var mainPage = new MainPage(MainPageViewModel);
        var chartPage = new ChartPage(ChartPageViewModel);
        var settingsPage = new SettingsPage(SettingVM);
        var listPage = new ListPage(ListPageVM);

        // Hinzufügen der Seiten zur TabbedPage
        Children.Add(mainPage);
        Children.Add(chartPage);
        Children.Add(settingsPage);
        Children.Add(listPage);
    }
}
