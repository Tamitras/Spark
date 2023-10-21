using Spark.ViewModels.Base;

namespace Spark.ViewModels.Settings
{
    public class SettingVM : BaseVM
    {
        public SettingVM()
        {
            // Default-Werte setzen
            PhotoCounter = 1;
            MeterReading = 123456.5;
        }

        private double _meterReading;
        public double MeterReading
        {
            get => _meterReading;
            set
            {
                _meterReading = value;
                OnPropertyChanged(nameof(MeterReading));
            }
        }

        private int _photoCounter;
        public int PhotoCounter
        {
            get => _photoCounter;
            set
            {
                _photoCounter = value;
                OnPropertyChanged(nameof(PhotoCounter));
            }
        }

        private double _pricePerKWh;
        public double PricePerKWh
        {
            get => _pricePerKWh;
            set
            {
                _pricePerKWh = value;
                OnPropertyChanged(nameof(PricePerKWh));
            }
        }
    }
}
