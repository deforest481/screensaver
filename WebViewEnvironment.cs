using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace WebViewScreensaver
{
    internal static class WebViewEnvironment
    {
        private static Task<CoreWebView2Environment>? _envTask;

        public static void SetupVirtualHost(CoreWebView2 coreWebView2)
        {
            string matrixFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "matrix");
            coreWebView2.SetVirtualHostNameToFolderMapping(
                RegistryHelper.VirtualHostName,
                matrixFolder,
                CoreWebView2HostResourceAccessKind.Allow);
        }

        public static Task<CoreWebView2Environment> GetEnvironmentAsync()
        {
            if (_envTask == null)
            {
                string userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "NascentMaker", "Screensaver", "WebView2");

                _envTask = CoreWebView2Environment.CreateAsync(null, userDataFolder);
            }
            return _envTask;
        }
    }
}
