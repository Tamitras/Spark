using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Spark.Class;
using Spark.Service;
using Spark.ViewModels.Base;
using Spark.ViewModels.List;
using Spark.ViewModels.Settings;

namespace Spark.ViewModels;

public class MainPageVM : BaseVM
{
    public ChartPageViewModel ChartPageViewModel { get; set; }
    public SettingVM SettingVM { get; set; }
    public ListPageVM ListPageVM { get; set; }

    public MainPageVM(ChartPageViewModel chartPageVM, SettingVM settingVM, ListPageVM listPageVM)
    {
        this.ChartPageViewModel = chartPageVM;
        this.SettingVM = settingVM;
        this.ListPageVM = listPageVM;

        this.ShowList = false;
        this.Photos.CollectionChanged += Photos_CollectionChanged;
    }

    private void Photos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        this.ShowList = Photos?.Count > 0;
    }
    private ObservableCollection<PhotoVM> _photos = new ObservableCollection<PhotoVM>();
    public ObservableCollection<PhotoVM> Photos
    {
        get => _photos;
        set
        {
            _photos = value;
            OnPropertyChanged(nameof(Photos));
        }
    }


    private bool _isVisible;
    public bool IsVisible
    {
        get => _isVisible;
        set
        {
            _isVisible = value;
            OnPropertyChanged(nameof(IsVisible));
        }
    }

    private string _currentMeterReading;
    public string CurrentMeterReading
    {
        get => _currentMeterReading;
        set
        {
            _currentMeterReading = value;
            OnPropertyChanged(nameof(CurrentMeterReading));
        }
    }

    private string _currentMeterInterpreted;
    public string CurrentMeterReadingInterpreted
    {
        get => _currentMeterInterpreted;
        set
        {
            _currentMeterInterpreted = value;
            OnPropertyChanged(nameof(CurrentMeterReadingInterpreted));
        }
    }

    private ObservableCollection<NumberWithScore> _htValue;
    public ObservableCollection<NumberWithScore> HTValues
    {
        get => _htValue;
        set
        {
            _htValue = value;
            OnPropertyChanged(nameof(HTValues));
        }
    }

    private ObservableCollection<NumberWithScore> _ntValue;
    public ObservableCollection<NumberWithScore> NTValues
    {
        get => _ntValue;
        set
        {
            _ntValue = value;
            OnPropertyChanged(nameof(NTValues));
        }
    }

    private NumberWithScore _selectedNTItem;
    public NumberWithScore SelectedNTItem
    {
        get => _selectedNTItem;
        set
        {
            if (_selectedNTItem != value)
            {
                _selectedNTItem = value;
                OnPropertyChanged(nameof(SelectedNTItem)); // Benachrichtigt die Benutzeroberfläche über die Änderung
            }
        }
    }

    private NumberWithScore _selectedHTItem;
    public NumberWithScore SelectedHTItem
    {
        get => _selectedHTItem;
        set
        {
            if (_selectedHTItem != value)
            {
                _selectedHTItem = value;
                OnPropertyChanged(nameof(_selectedHTItem)); // Benachrichtigt die Benutzeroberfläche über die Änderung
            }
        }
    }

    private string _status;
    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged(nameof(Status));
        }
    }

    private bool _showList;
    public bool ShowList
    {
        get => false;
        //get => _showList;
        set
        {
            _showList = value;
            OnPropertyChanged(nameof(ShowList));
        }
    }

    async Task AnalyzeImage(PhotoVM photo)
    {
        this.IsVisible = true;

        GVisionService vision = new GVisionService();

        try
        {
            Console.WriteLine("Extrahiere TExt aus Bild!!");

            if(photo.ByteArrayThumbnail == null)
            {
                //TODO: break out routine needed (otherwise endless loop)
                Console.WriteLine("ByteArrayThumbnail ist null --> 500ms delay");
                await Task.Delay(500);
                await AnalyzeImage(photo);
            }
            else
            {
                var extractedText = await vision.RecognizeTextAsync(photo.ByteArrayThumbnail);

                var rankedList = Helper.GetRankedPotentialNumbers(extractedText, SettingVM.MeterReading);

                photo.NewMeterReading = rankedList.FirstOrDefault()?.Number ?? 00000.0;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        this.IsVisible = false;
    }

    public async Task ProcessPhotoVMAsync(PhotoVM photoVM)
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Hinzufügen des Bildes in die Liste
            this.Photos.Add(photoVM);

            // Überprüfen Sie die Eingabe
            if (photoVM == null || photoVM.OriginalFile == null)
                throw new ArgumentNullException(nameof(photoVM));

            // Setzen des Status auf 'Converting'
            photoVM.Status = ProcessingStatus.Converting;
            await Task.Run(async () => { await ConvertToThumbnail(photoVM); });
            Console.WriteLine("ConvertToThumbnail abgeschlossen");
            photoVM.Status = ProcessingStatus.Processing;


            Console.WriteLine("AnalyzeImage start");
            await Task.Run(async () => { await AnalyzeImage(photoVM); });
            Console.WriteLine("AnalyzeImage abgeschlossen");

            // Setzen des Status auf 'Done', wenn die Bearbeitung abgeschlossen ist
            photoVM.Status = ProcessingStatus.Done;

            stopwatch.Stop();
            photoVM.ProcessingTime = stopwatch.Elapsed;

            Console.WriteLine($"Benötigte Zeit für Bild {photoVM.Name}: {photoVM.ProcessingTime}");
        }
        catch (Exception ex)
        {
            photoVM.ErrorMessage = $"Thumbnail Error: {ex}";
        }
    }

    private async Task ConvertToThumbnail(PhotoVM photoVM)
    {
        try
        {
            photoVM.Thumbnail = await Helper.ConvertToThumbnail(photoVM.ByteArrayOriginal);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Converting to Thumbnail: {ex}");
        }
    }
}
