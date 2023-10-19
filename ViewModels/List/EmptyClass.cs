using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Spark.ViewModels.List;

public class ObservableItemCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
{
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        base.OnCollectionChanged(e);

        if (e.NewItems != null)
        {
            foreach (INotifyPropertyChanged newItem in e.NewItems)
            {
                newItem.PropertyChanged += OnItemPropertyChanged;
            }
        }

        if (e.OldItems != null)
        {
            foreach (INotifyPropertyChanged oldItem in e.OldItems)
            {
                oldItem.PropertyChanged -= OnItemPropertyChanged;
            }
        }
    }

    private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // Hier könnte eine Aktion erfolgen, wenn sich eine Eigenschaft eines Elements ändert
    }
}

