using System.Runtime.InteropServices;

namespace Gapotchenko.FX.Threading
{
    static class MemoryModel
    {
        static MemoryModel()
        {
            WritesCanBeReordered =
                RuntimeInformation.ProcessArchitecture switch
                {
                    Architecture.X86 or Architecture.X64 => false,
                    Architecture.Arm or Architecture.Arm64 => true,
                    _ => true // to be on a safe side
                };
        }

        internal static bool WritesCanBeReordered { get; }
    }
}
