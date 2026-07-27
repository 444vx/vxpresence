using System;
using System.Runtime.InteropServices;

namespace VxPresence.Native
{
    public static class IdleDetector
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }

        [DllImport("user32.dll")]
        private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        public static double GetIdleSeconds()
        {
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint envTicks = (uint)Environment.TickCount;
                uint idleTicks = envTicks - lastInputInfo.dwTime;
                return idleTicks / 1000.0;
            }

            return 0;
        }
    }
}