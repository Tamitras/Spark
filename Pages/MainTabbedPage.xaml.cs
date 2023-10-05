// In MainTabbedPage.xaml.cs
using Spark.Pages;
using Spark.ViewModels;

public partial class MainTabbedPage : TabbedPage
{
    public MainPageViewModel MainPageViewModel { get; set; }
    public ChartPageViewModel ChartPageViewModel { get; set; }

    public MainTabbedPage()
    {
        // Erstellen der ViewModels
        MainPageViewModel = new MainPageViewModel(new ChartPageViewModel());
        ChartPageViewModel = MainPageViewModel.ChartPageViewModel;

        // Erstellen der Seiten mit den ViewModels
        var mainPage = new MainPage(MainPageViewModel);
        var chartPage = new ChartPage(ChartPageViewModel);

        // Hinzufügen der Seiten zur TabbedPage
        Children.Add(mainPage);
        Children.Add(chartPage);
    }
}
