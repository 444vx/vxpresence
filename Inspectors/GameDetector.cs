using System.Diagnostics;
using System.Linq;

namespace VxPresence.Inspectors
{
    public static class GameDetector
    {
        public static bool IsGraphicsHeavy(Process process)
        {
            try
            {
                var modules = process.Modules
                    .OfType<ProcessModule>()
                    .Select(m => m.ModuleName.ToLower());

                return modules.Any(m => 
                    m.Contains("d3d11") || 
                    m.Contains("d3d12") || 
                    m.Contains("vulkan") || 
                    m.Contains("opengl32"));
            }
            catch
            {
                return false;
            }
        }
    }
}
