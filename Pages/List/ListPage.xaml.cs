using CommunityToolkit.Maui.Views;
using Spark.ViewModels;
using Spark.ViewModels.List;
using Syncfusion.Maui.Popup;

namespace Spark.Pages.List;

public partial class ListPage : ContentPage
{
    public ListPageVM ListPageVM { get; set; }


    public ListPage(ListPageVM vm)
    {
        InitializeComponent();

        ListPageVM = vm;

        this.BindingContext = ListPageVM;
    }

    void OnDeleteSwipeItemInvoked(Object sender, EventArgs e)
    {
        Console.WriteLine("SwipeDELETE");
    }

    void OnMoreSwipeItemInvoked(Object sender, EventArgs e)
    {
        // Das aktuelle Foto von SwipeItem erhalten
        var swipeItem = sender as SwipeItem;
        var photo = swipeItem.BindingContext as PhotoVM;

        // Das BindingContext des Popups setzen, um das Foto anzuzeigen und zu bearbeiten
        popupEdit.BindingContext = photo;

        // Das Popup anzeigen
        popupEdit.Show();
    }
}
