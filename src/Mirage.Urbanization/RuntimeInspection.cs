using System;

namespace Mirage.Urbanization
{
    public static class RuntimeInspection
    {
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
