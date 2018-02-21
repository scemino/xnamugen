using System;

namespace xnaMugen.Diagnostics
{
    internal static class OperatingSystemExtension
    {
        public static bool IsWindows(this OperatingSystem os)
        {
            switch (os.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return true;
                default:
                    return false;
            }
        }
    }
}