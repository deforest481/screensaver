using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WebViewScreensaver
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            var app = new App();
            app.DispatcherUnhandledException += OnUnhandledException;

            if (args.Length == 0)
            {
                var config = new ConfigWindow();
                config.ShowDialog();
                return;
            }

            string arg = args[0].Trim().ToLowerInvariant();

            if (arg.StartsWith("/c"))
            {
                var config = new ConfigWindow();
                config.ShowDialog();
            }
            else if (arg == "/s")
            {
                RunScreensaver(app);
                app.Run();
            }
            else if (arg == "/p" && args.Length >= 2)
            {
                IntPtr hwnd = new IntPtr(long.Parse(args[1]));

                var previewWin = new PreviewWindow(hwnd);
                previewWin.Show();
                app.Run();
            }
            else
            {
                // Unknown arg — show config as fallback
                var config = new ConfigWindow();
                config.ShowDialog();
            }
        }

        private static void RunScreensaver(App app)
        {
            var screens = System.Windows.Forms.Screen.AllScreens;
            var windows = new List<ScreensaverWindow>();

            foreach (var screen in screens)
            {
                var win = new ScreensaverWindow(screen.Bounds);
                win.Show();
                windows.Add(win);
            }

            // Install hooks only after all WebView2s are ready + 500ms grace period
            // to absorb spurious WM_MOUSEMOVE from window creation.
            var readyTasks = windows.Select(w => w.ReadyTask).ToArray();
            Task.WhenAll(readyTasks).ContinueWith(_ =>
            {
                return Task.Delay(500);
            }).Unwrap().ContinueWith(_ =>
            {
                app.Dispatcher.Invoke(() => GlobalHooks.Install(app));
            });
        }

        private static void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.ToString(), "WebViewScreensaver Error",
                MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}
