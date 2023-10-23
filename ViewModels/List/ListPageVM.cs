using System.Collections.ObjectModel;
using Spark.ViewModels.Base;

namespace Spark.ViewModels.List;

public class ListPageVM : BaseVM
{
    /// <summary>
    /// Constructor with photos init
    /// </summary>
    /// <param name="photos"></param>
	public ListPageVM()
    {

    }

    public void Init(ObservableCollection<PhotoVM> photos)
    {
        this.Photos = photos;
    }


    private ObservableCollection<PhotoVM> _photos;

    public ObservableCollection<PhotoVM> Photos
    {
        get => _photos;
        set
        {
            _photos = value;
            OnPropertyChanged(nameof(Photos));
        }
    }
}
