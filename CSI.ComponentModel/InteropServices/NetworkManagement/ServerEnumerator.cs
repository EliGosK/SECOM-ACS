namespace CSI.InteropServices.NetworkManagement
{
    using CSI.InteropServices;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class ServerEnumerator : IEnumerator
    {
        protected int currentItem;
        protected string currentServerName;
        protected uint itemCount;
        protected static int SERVER_INFO_101_SIZE = Marshal.SizeOf(typeof(Win32API.SERVER_INFO_101));
        protected IntPtr serverInfoPtr;

        protected internal ServerEnumerator(ServerType serverType) : this(serverType, null)
        {
        }

        protected internal ServerEnumerator(ServerType serverType, string domainName)
        {
            uint level = 0x65;
            uint maxValue = uint.MaxValue;
            uint entriesread = 0;
            uint totalentries = 0;
            this.Reset();
            this.serverInfoPtr = IntPtr.Zero;
            Win32API.NetServerEnum(IntPtr.Zero, level, ref this.serverInfoPtr, maxValue, ref entriesread, ref totalentries, (uint) serverType, domainName, IntPtr.Zero);
            this.itemCount = entriesread;
        }

        ~ServerEnumerator()
        {
            if (!this.serverInfoPtr.Equals(IntPtr.Zero))
            {
                Win32API.NetApiBufferFree(this.serverInfoPtr);
                this.serverInfoPtr = IntPtr.Zero;
            }
        }

        public bool MoveNext()
        {
            bool flag = false;
            if (++this.currentItem < this.itemCount)
            {
                int num = this.serverInfoPtr.ToInt32() + (SERVER_INFO_101_SIZE * this.currentItem);
                Win32API.SERVER_INFO_101 server_info_ = (Win32API.SERVER_INFO_101) Marshal.PtrToStructure(new IntPtr(num), typeof(Win32API.SERVER_INFO_101));
                this.currentServerName = Marshal.PtrToStringAuto(server_info_.lpszServerName);
                flag = true;
            }
            return flag;
        }

        public void Reset()
        {
            this.currentItem = -1;
            this.currentServerName = null;
        }

        public object Current
        {
            get
            {
                return this.currentServerName;
            }
        }
    }
}

