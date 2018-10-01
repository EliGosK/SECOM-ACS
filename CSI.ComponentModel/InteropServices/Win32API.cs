namespace CSI.InteropServices
{
    using System;
    using System.Runtime.InteropServices;

    internal class Win32API
    {
        [DllImport("netapi32.dll")]
        internal static extern void NetApiBufferFree(IntPtr bufptr);
        [DllImport("netapi32.dll")]
        internal static extern uint NetServerEnum(IntPtr ServerName, uint level, ref IntPtr siPtr, uint prefmaxlen, ref uint entriesread, ref uint totalentries, uint servertype, [MarshalAs(UnmanagedType.LPWStr)] string domain, IntPtr resumeHandle);
        [DllImport("netapi32.dll")]
        internal static extern uint NetServerGetInfo([MarshalAs(UnmanagedType.LPWStr)] string ServerName, int level, ref IntPtr buffPtr);

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        internal struct SERVER_INFO_101
        {
            public int dwPlatformID;
            public IntPtr lpszServerName;
            public int dwVersionMajor;
            public int dwVersionMinor;
            public int dwType;
            public IntPtr lpszComment;
        }
    }
}

