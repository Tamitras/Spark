using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spark.ViewModels;

public class MainPageViewModel : INotifyPropertyChanged
{
    public ChartPageViewModel ChartPageViewModel { get; set; }

    public MainPageViewModel(ChartPageViewModel chartPageVM)
    {
        this.ChartPageViewModel = chartPageVM;
    }

    private string _currentMeterReading;
    public string CurrentMeterReading
    {
        get => _currentMeterReading;
        set
        {
            _currentMeterReading = value;
            OnPropertyChanged();
        }
    }

    private string _currentMeterInterpreted;
    public string CurrentMeterReadingInterpreted
    {
        get => _currentMeterInterpreted;
        set
        {
            _currentMeterInterpreted = value;
            OnPropertyChanged();
        }
    }

    private string _status;
    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
