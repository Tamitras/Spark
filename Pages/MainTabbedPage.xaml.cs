namespace Spark.Pages;
using Spark.ViewModels;

public partial class MainTabbedPage : TabbedPage
{
    public MainPageVM MainPageViewModel { get; set; }
    public ChartPageViewModel ChartPageViewModel { get; set; }

    public MainTabbedPage()
    {
        InitializeComponent();

        // Erstellen der ViewModels
        MainPageViewModel = new MainPageVM(new ChartPageViewModel());
        ChartPageViewModel = MainPageViewModel.ChartPageViewModel;

        // Erstellen der Seiten mit den ViewModels
        var mainPage = new MainPage(MainPageViewModel);
        var chartPage = new ChartPage(ChartPageViewModel);

        // Hinzufügen der Seiten zur TabbedPage
        Children.Add(mainPage);
        Children.Add(chartPage);
    }
}
