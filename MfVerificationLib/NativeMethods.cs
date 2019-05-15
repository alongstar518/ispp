using System;
using System.Runtime.InteropServices;


namespace MfVerificationLib
{
    public static class NativeMethods
    {
        private const string DLLFILE = "MFUIfocuslib.dll";

        [DllImport(DLLFILE)]
        public static extern void GetFocusImg(byte[] inputPathBytes, byte[] outputPathBytes, [MarshalAs(UnmanagedType.U1)]bool lineItem, int lineItemWide, int[] bgr, [MarshalAs(UnmanagedType.U1)]bool changeBackground);

        [DllImport(DLLFILE)]
        [return: MarshalAs(UnmanagedType.U1)]
        public static extern bool CompareImage(byte[] img1PathBytes, byte[] img2PathBytes);
    }
}
