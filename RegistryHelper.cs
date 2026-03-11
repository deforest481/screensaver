using Microsoft.Win32;

namespace WebViewScreensaver
{
    public static class RegistryHelper
    {
        private const string KeyPath = @"Software\NascentMaker\Screensaver";
        private const string ValueName = "Url";
        public const string DefaultUrl = "https://matrix.screensaver.local/index.html?version=resurrections";
        public const string VirtualHostName = "matrix.screensaver.local";
        public const string RemoteFallbackUrl = "https://rezmason.github.io/matrix/?version=resurrections";

        public static string GetUrl()
        {
            using var key = Registry.CurrentUser.OpenSubKey(KeyPath);
            var stored = key?.GetValue(ValueName) as string;
            // Migrate old remote default to embedded default
            if (string.IsNullOrEmpty(stored) || stored == RemoteFallbackUrl)
                return DefaultUrl;
            return stored!;
        }

        /// <summary>Returns null if no custom URL is set (i.e. using the embedded default).</summary>
        public static string? GetCustomUrl()
        {
            using var key = Registry.CurrentUser.OpenSubKey(KeyPath);
            var stored = key?.GetValue(ValueName) as string;
            if (string.IsNullOrEmpty(stored) || stored == RemoteFallbackUrl || stored == DefaultUrl)
                return null;
            return stored;
        }

        public static void SetUrl(string url)
        {
            using var key = Registry.CurrentUser.CreateSubKey(KeyPath);
            key.SetValue(ValueName, url);
        }

        public static void DeleteUrl()
        {
            using var key = Registry.CurrentUser.OpenSubKey(KeyPath, writable: true);
            key?.DeleteValue(ValueName, throwOnMissingValue: false);
        }
    }
}
