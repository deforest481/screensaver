using System;
using System.Windows;
using System.Windows.Interop;

namespace WebViewScreensaver
{
    public partial class PreviewWindow : Window
    {
        private readonly IntPtr _previewHwnd;

        public PreviewWindow(IntPtr previewHwnd)
        {
            _previewHwnd = previewHwnd;
            InitializeComponent();
            SourceInitialized += OnSourceInitialized;
            Loaded += OnLoaded;
        }

        private void OnSourceInitialized(object sender, EventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;

            // Reparent WPF window into the preview panel HWND
            NativeMethods.SetParent(hwnd, _previewHwnd);

            // Switch from WS_POPUP to WS_CHILD so it's a proper child window
            int style = NativeMethods.GetWindowLong(hwnd, NativeMethods.GWL_STYLE);
            style = (style & ~NativeMethods.WS_POPUP) | NativeMethods.WS_CHILD | NativeMethods.WS_VISIBLE;
            NativeMethods.SetWindowLong(hwnd, NativeMethods.GWL_STYLE, style);

            // Size to fill the preview panel
            NativeMethods.GetClientRect(_previewHwnd, out var rect);
            int w = rect.Width  > 0 ? rect.Width  : 200;
            int h = rect.Height > 0 ? rect.Height : 150;
            NativeMethods.MoveWindow(hwnd, 0, 0, w, h, true);

            // Update WPF logical size (96 dpi assumed for preview — it's tiny anyway)
            Width  = w;
            Height = h;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var env = await WebViewEnvironment.GetEnvironmentAsync();
                await WebView.EnsureCoreWebView2Async(env);
                WebViewEnvironment.SetupVirtualHost(WebView.CoreWebView2);
                WebView.Source = new Uri(RegistryHelper.GetUrl());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Preview WebView2 init failed: {ex}");
            }
        }
    }
}
