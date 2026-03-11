using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;

namespace WebViewScreensaver
{
    public partial class ScreensaverWindow : Window
    {
        private readonly Rectangle _screenBounds;
        private readonly TaskCompletionSource<bool> _readyTcs = new TaskCompletionSource<bool>();

        /// <summary>Completes when WebView2 has finished initializing.</summary>
        public Task ReadyTask => _readyTcs.Task;

        public ScreensaverWindow(Rectangle screenBounds)
        {
            _screenBounds = screenBounds;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            // Position to the target screen.
            // PresentationSource may not be available yet for off-screen windows;
            // fall back to 96 dpi (scale=1) which is corrected by PerMonitorV2 manifest.
            var source = PresentationSource.FromVisual(this);
            double dpiScaleX = source?.CompositionTarget?.TransformFromDevice.M11 ?? 1.0;
            double dpiScaleY = source?.CompositionTarget?.TransformFromDevice.M22 ?? 1.0;

            Left   = _screenBounds.Left   * dpiScaleX;
            Top    = _screenBounds.Top    * dpiScaleY;
            Width  = _screenBounds.Width  * dpiScaleX;
            Height = _screenBounds.Height * dpiScaleY;

            try
            {
                var env = await WebViewEnvironment.GetEnvironmentAsync();
                await WebView.EnsureCoreWebView2Async(env);
                WebViewEnvironment.SetupVirtualHost(WebView.CoreWebView2);
                string url = RegistryHelper.GetUrl();
                WebView.Source = new Uri(url);
                _readyTcs.TrySetResult(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WebView2 init failed: {ex}");
                _readyTcs.TrySetException(ex);
            }
        }
    }
}
