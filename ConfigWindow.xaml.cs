using System.Windows;

namespace WebViewScreensaver
{
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UrlTextBox.Text = RegistryHelper.GetCustomUrl() ?? string.Empty;
            UrlTextBox.SelectAll();
            UrlTextBox.Focus();
        }

        private void OnOkClick(object sender, RoutedEventArgs e)
        {
            string url = UrlTextBox.Text.Trim();
            if (string.IsNullOrEmpty(url))
                RegistryHelper.DeleteUrl();
            else
                RegistryHelper.SetUrl(url);

            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
