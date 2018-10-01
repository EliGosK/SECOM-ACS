namespace CSI.InteropServices.NetworkManagement
{
    using CSI.InteropServices;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class Servers : IEnumerable
    {
        private string domainName;
        private ServerType serverType;

        public Servers()
        {
            this.Type = ServerType.None;
        }

        public Servers(ServerType aServerType)
        {
            this.Type = aServerType;
        }

        public IEnumerator GetEnumerator()
        {
            return new ServerEnumerator(this.serverType, this.domainName);
        }

        public static ServerType GetServerType(string serverName)
        {
            ServerType none = ServerType.None;
            IntPtr zero = IntPtr.Zero;
            if (Win32API.NetServerGetInfo(serverName, 0x65, ref zero) != 0)
            {
                Win32API.SERVER_INFO_101 server_info_ = (Win32API.SERVER_INFO_101) Marshal.PtrToStructure(zero, typeof(Win32API.SERVER_INFO_101));
                none = (ServerType) server_info_.dwType;
                Win32API.NetApiBufferFree(zero);
                zero = IntPtr.Zero;
            }
            return none;
        }

        public string DomainName
        {
            get
            {
                return this.domainName;
            }
            set
            {
                this.domainName = value;
            }
        }

        public ServerType Type
        {
            get
            {
                return this.serverType;
            }
            set
            {
                this.serverType = value;
            }
        }
    }
}

