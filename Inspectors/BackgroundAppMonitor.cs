using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VxPresence.Native;

namespace VxPresence.Inspectors
{
    public class BackgroundAppMonitor
    {
        private static readonly string[] SystemProcesses = { "explorer", "SearchHost", "StartMenuExperienceHost", "TextInputActionHost", "dwmapi" };

        public List<string> GetActiveBackgroundApps(int limit = 4)
        {
            var apps = new List<string>();

            Win32.EnumWindows((hWnd, lParam) =>
            {
                if (Win32.IsWindowVisible(hWnd))
                {
                    StringBuilder sb = new StringBuilder(256);
                    Win32.GetWindowText(hWnd, sb, 256);
                    string title = sb.ToString().Trim();

                    if (!string.IsNullOrEmpty(title) && !title.Equals("Program Manager"))
                    {
                        Win32.GetWindowThreadProcessId(hWnd, out uint procId);
                        try
                        {
                            var proc = Process.GetProcessById((int)procId);
                            if (!apps.Contains(proc.ProcessName) && !SystemProcesses.Contains(proc.ProcessName, StringComparer.OrdinalIgnoreCase))
                            {
                                apps.Add(proc.ProcessName);
                            }
                        }
                        catch { }
                    }
                }
                return true;
            }, IntPtr.Zero);

            return apps.Take(limit).ToList();
        }
    }
}