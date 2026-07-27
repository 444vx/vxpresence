using Microsoft.Win32;
using System;
using System.Diagnostics;

namespace VxPresence.Services
{
    public static class AutostartManager
    {
        private const string APP_NAME = "VxPresence";

        public static bool IsAutostartEnabled()
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);
            return key?.GetValue(APP_NAME) != null;
        }

        public static void SetAutostart(bool enable)
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
            if (key == null) return;

            if (enable)
            {
                string? exePath = Process.GetCurrentProcess().MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath))
                {
                    key.SetValue(APP_NAME, $"\"{exePath}\"");
                }
            }
            else
            {
                key.DeleteValue(APP_NAME, false);
            }
        }
    }
}