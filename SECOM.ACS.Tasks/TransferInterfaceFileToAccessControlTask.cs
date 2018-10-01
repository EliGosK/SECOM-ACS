using CSI.ComponentModel;
using CSI.IO;
using SECOM.ACS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    [AcsTaskAttribute(CanEdit = false, CanRun = false)]
    public class TransferInterfaceFileToAccessControlTask : AcsTaskBase<TransferInterfaceFileToAccessControlTaskOptions>, IDisposable
    {
        public TransferInterfaceFileToAccessControlTask() 
            : base("ACP040", "Transfer interface file to Access Control System")
        {
           
        }

        protected override object ExecuteTask(TransferInterfaceFileToAccessControlTaskOptions options)
        {
            string sourceFolder = FileHelper.GetApplicationFullPath(options.SourceFolder);
            string targetFolder = FileHelper.GetApplicationFullPath(options.TargetFolder);

            var dir = new DirectoryInfo(sourceFolder);
            // Find interface file to import.
            var file = dir.EnumerateFiles("*", SearchOption.TopDirectoryOnly).OrderBy(t => t.CreationTime).FirstOrDefault();
            if (file == null) {
                // No interface file to operate.
                OnProgress(new TaskProgressEventArgs($"No interface file operate to Access Control System in folder {dir.FullName}."));
                return null;
            }

            var targetFile = Path.Combine(targetFolder, options.TargetFileName);

            if (options.EnabledNetworkShareAuthenticate)
            {
                using (var n = NetworkShareAccesser.Access(options.RemoteComputerName, options.UserName, options.Password))
                {
                    if (File.Exists(targetFile))
                    {
                        OnProgress(new TaskProgressEventArgs($"Target file {file.FullName} is exists. Skip copy file to shared folder."));
                    }
                    else
                    {
                        OnProgress(new TaskProgressEventArgs($"Copy interface file {file.FullName} to {targetFile}"));
                        file.CopyTo(targetFile, true);
                    }
                }
            }
            else
            {
                if (File.Exists(targetFile))
                {
                    OnProgress(new TaskProgressEventArgs($"Target file {file.FullName} is exists. Skip copy file to shared folder."));
                }
                else {
                    OnProgress(new TaskProgressEventArgs($"Copy interface file {file.FullName} to {targetFile}"));
                    file.CopyTo(targetFile, true);

                    if (options.EnabledArchive)
                    {
                        // Move acs file to history folder.
                        //var historyFileName = String.Format(options.ArchiveFileName, DateTime.Now);
                        var historyFileName = file.Name;
                        var historyFile = Path.Combine(FileHelper.GetApplicationFullPath(options.ArchiveFolder), historyFileName);
                        DirectoryHelper.EnsureDirectoryCreated(historyFile);
                        if (File.Exists(historyFile))
                        {
                            File.Delete(historyFile);
                        }
                        file.MoveTo(historyFile);
                        OnProgress(new TaskProgressEventArgs($"Archive acs interface file to history folder. Archrive file {historyFile}"));
                    }
                }
            }
            return targetFile;
        }
        
    }
}
