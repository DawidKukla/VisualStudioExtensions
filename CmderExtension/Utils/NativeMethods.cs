using System;
using System.Runtime.InteropServices;

namespace CmderExtension.Utils
{
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
    }
}
