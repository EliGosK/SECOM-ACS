namespace CSI.InteropServices.NetworkManagement
{
    using System;

    [Flags]
    public enum ServerType : long
    {
        AFP = 0x40L,
        All = 0xffffffffL,
        AlternateXPort = 0x20000000L,
        BackupBrowser = 0x20000L,
        ClusterNT = 0x1000000L,
        DCE = 0x10000000L,
        DFS = 0x800000L,
        Dialin = 0x400L,
        DomainBackupController = 0x10L,
        DomainController = 8L,
        DomainEnum = 0x80000000L,
        DomainMaster = 0x80000L,
        DomainMember = 0x100L,
        ListOnly = 0x40000000L,
        MasterBrowser = 0x40000L,
        MFPN = 0x4000L,
        None = 0L,
        Novell = 0x80L,
        NT = 0x1000L,
        NTServer = 0x8000L,
        OSF = 0x100000L,
        PotentialBrowser = 0x10000L,
        PrintQueue = 0x200L,
        Server = 2L,
        SQLServer = 4L,
        TerminalServer = 0x2000000L,
        TimeSource = 0x20L,
        Unix = 0x800L,
        VMS = 0x200000L,
        WFW = 0x2000L,
        Windows = 0x400000L,
        Workstation = 1L,
        Xenix = 0x800L
    }
}

