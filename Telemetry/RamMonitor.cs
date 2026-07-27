using System.Runtime.InteropServices;
using VxPresence.Native;

namespace VxPresence.Telemetry
{
    public class RamMonitor
    {
        public (int UsedPercentage, string FormattedText) GetUsage()
        {
            var memStatus = new Win32.MEMORYSTATUSEX
            {
                dwLength = (uint)Marshal.SizeOf(typeof(Win32.MEMORYSTATUSEX))
            };

            if (Win32.GlobalMemoryStatusEx(ref memStatus))
            {
                ulong totalGb = memStatus.ullTotalPhys / (1024 * 1024 * 1024);
                ulong usedGb = (memStatus.ullTotalPhys - memStatus.ullAvailPhys) / (1024 * 1024 * 1024);
                return ((int)memStatus.dwMemoryLoad, $"{usedGb}/{totalGb} GB");
            }

            return (0, "N/A");
        }
    }
}