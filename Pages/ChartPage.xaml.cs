using Spark.ViewModels;

namespace Spark.Pages;

public partial class ChartPage : ContentPage
{
    public ChartPageViewModel ViewModel { get; set; }

    public ChartPage(ChartPageViewModel vm)
	{
        InitializeComponent();
        this.ViewModel = vm;
		BindingContext = ViewModel;
	}
}
