using Mopups.Services;

namespace Spark.Pages.Popups
{
    public partial class ValidationPopup
    {
        public string HtValue { get; set; }
        public string NtValue { get; set; }

        // Definieren Sie ein Ereignis, das ausgelöst wird, wenn der Benutzer auf OK klickt.
        public event Action<bool> UserResponse;

        public ValidationPopup(string htValue, string ntValue)
        {
            InitializeComponent();
            HtValue = htValue;
            NtValue = ntValue;

            HtEntry.Text = HtValue;
            NtEntry.Text = NtValue;
        }

        private async void OnOkButtonClicked(object sender, EventArgs e)
        {
            UserResponse?.Invoke(true);  // true bedeutet, dass der Benutzer auf OK geklickt hat.
            await MopupService.Instance.PopAsync();
        }

        private async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            UserResponse?.Invoke(false);  // false bedeutet, dass der Benutzer auf Abbrechen geklickt hat.
            await MopupService.Instance.PopAsync();
        }
    }
}
