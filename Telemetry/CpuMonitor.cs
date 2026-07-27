using System;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace VxPresence.Telemetry
{
    [SupportedOSPlatform("windows")]
    public class CpuMonitor
    {
        private readonly PerformanceCounter _cpuCounter;

        public CpuMonitor()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _cpuCounter.NextValue();
        }

        public float GetUsage()
        {
            try
            {
                return _cpuCounter.NextValue();
            }
            catch
            {
                return 0f;
            }
        }
    }
}
