using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using VxPresence.Native;

namespace VxPresence.Inspectors
{
    public class AppInfo
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public string WindowTitle { get; set; } = string.Empty;
        public bool IsMedia { get; set; } = false;
    }

    public class AppInspector
    {
        private readonly Dictionary<string, string> _browsers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "chrome", "Google Chrome" },
            { "firefox", "Mozilla Firefox" },
            { "msedge", "Microsoft Edge" },
            { "opera", "Opera" },
            { "opera_gx", "Opera GX" },
            { "brave", "Brave Browser" },
            { "vivaldi", "Vivaldi" },
            { "waterfox", "Waterfox" },
            { "librewolf", "LibreWolf" },
            { "arc", "Arc Browser" }
        };

        private readonly Dictionary<string, string> _mediaApps = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "spotify", "Spotify" },
            { "tidal", "TIDAL" },
            { "vlc", "VLC Media Player" },
            { "wmplayer", "Windows Media Player" },
            { "foobar2000", "foobar2000" },
            { "musicbee", "MusicBee" },
            { "applemusic", "Apple Music" }
        };

        public AppInfo Inspect(Process process)
        {
            string procName = process.ProcessName.ToLower();
            string rawTitle = GetWindowTitle(process.MainWindowHandle);

            // 1. Dedykowane aplikacje muzyczne
            if (_mediaApps.TryGetValue(procName, out string? mediaAppName))
            {
                return new AppInfo
                {
                    DisplayName = mediaAppName,
                    Category = "🎧 Media",
                    ProcessName = process.ProcessName,
                    WindowTitle = string.IsNullOrWhiteSpace(rawTitle) ? "Playing Music/Video" : rawTitle,
                    IsMedia = true
                };
            }

            // 2. Przeglądarki
            if (_browsers.TryGetValue(procName, out string? browserDisplayName))
            {
                string cleanTabTitle = CleanBrowserTitle(rawTitle);
                bool isWebMedia = IsWebMedia(rawTitle);

                return new AppInfo
                {
                    DisplayName = browserDisplayName,
                    Category = isWebMedia ? "🎵 Web Media" : "🌐 Browser",
                    ProcessName = process.ProcessName,
                    WindowTitle = string.IsNullOrWhiteSpace(cleanTabTitle) ? "Browsing the Web" : cleanTabTitle,
                    IsMedia = isWebMedia
                };
            }

            // 3. Zwykła aplikacja
            string appName = char.ToUpper(procName[0]) + procName.Substring(1);
            return new AppInfo
            {
                DisplayName = appName,
                Category = "💻 App",
                ProcessName = process.ProcessName,
                WindowTitle = string.IsNullOrWhiteSpace(rawTitle) ? appName : rawTitle,
                IsMedia = false
            };
        }

        private bool IsWebMedia(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return false;
            string lower = title.ToLower();

            return lower.Contains("youtube") ||
                   lower.Contains("spotify") ||
                   lower.Contains("soundcloud") ||
                   lower.Contains("netflix") ||
                   lower.Contains("twitch") ||
                   lower.Contains("prime video") ||
                   lower.Contains("disney+");
        }

        private string GetWindowTitle(IntPtr hWnd)
        {
            if (hWnd == IntPtr.Zero) return string.Empty;

            StringBuilder sb = new StringBuilder(256);
            Win32.GetWindowText(hWnd, sb, sb.Capacity);
            return sb.ToString().Trim();
        }

        private string CleanBrowserTitle(string fullTitle)
        {
            if (string.IsNullOrWhiteSpace(fullTitle)) return string.Empty;

            string cleaned = fullTitle;

            int dashIndex = cleaned.LastIndexOf(" - ");
            if (dashIndex != -1)
                cleaned = cleaned.Substring(0, dashIndex);

            int emDashIndex = cleaned.LastIndexOf(" — ");
            if (emDashIndex != -1)
                cleaned = cleaned.Substring(0, emDashIndex);

            return cleaned.Trim();
        }
    }
}