using Spark.ViewModels.Base;

namespace Spark.ViewModels.Settings
{
    public class SettingVM : BaseVM
    {
        public SettingVM()
        {
            // Default-Werte setzen
            PhotoCounter = 1;
            DigitCount = 6;
        }

        private int _digitCount;
        public int DigitCount
        {
            get => _digitCount;
            set
            {
                _digitCount = value;
                OnPropertyChanged(nameof(DigitCount));
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
