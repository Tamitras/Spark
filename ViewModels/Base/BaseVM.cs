using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Spark.ViewModels.Base
{
	public abstract class BaseVM : INotifyPropertyChanged
    {
		public BaseVM()
		{
		}

        // Methode PrintAllInfo
        public virtual void PrintAllInfo()
        {
            // Drucken Sie die Informationen aller Eigenschaften auf die Konsole.
            // Reflection wird verwendet, um durch alle Eigenschaften der Klasse zu iterieren und ihre Werte zu drucken.
            Type type = this.GetType();
            foreach (var prop in type.GetProperties())
            {
                string propName = prop.Name;
                object propValue = prop.GetValue(this);
                Console.WriteLine($"{propName}: {propValue}");
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _creationDate;
        public string CreationDate
        {
            get => _creationDate ?? DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            set
            {
                _creationDate = value;
                OnPropertyChanged(nameof(CreationDate));
            }
        }

        private string _changeDate;
        public string ChangeDate
        {
            get => _changeDate ?? DateTime.Now.ToString("MM/dd/yyyy HH:mm");
            set
            {
                _changeDate = value;
                OnPropertyChanged(nameof(ChangeDate));
            }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            //Console.WriteLine($"PropertyName Changed: {propertyName}");
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Update()
        {
            this.Name = "Updated";
        }
    }
}

