using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Spark.Class;

namespace Spark.ViewModels;

public class ChartPageViewModel : INotifyPropertyChanged
{

    public ChartPageViewModel()
    {
        this.SparkInfo = new ObservableCollection<SparkInfo>();
    }

    private ObservableCollection<SparkInfo> _sparkInfo;
    public ObservableCollection<SparkInfo> SparkInfo
    {
        get => _sparkInfo;
        set
        {
            _sparkInfo = value;
            OnPropertyChanged();
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
