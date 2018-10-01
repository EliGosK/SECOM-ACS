using System;
using System.IO;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace CSI.IO
{
    /// <summary>
    /// 
    /// </summary>
    public class FileHelper
    {
        public static void CopyDirectory(string pathFrom, string pathTo)
        {
            CopyDirectory(pathFrom, pathTo, null);
        }

        public static void CopyDirectory(string pathFrom, string pathTo, string filter)
        {
            CopyDirectory(pathFrom, pathTo, filter, 0);
        }

        private static void CopyDirectory(string pathFrom, string pathTo, string filter, int level)
        {
            if (level <= 3)
            {
                if (!Directory.Exists(pathTo))
                {
                    Directory.CreateDirectory(pathTo);
                }
                foreach (string str in Directory.GetFiles(pathFrom, filter))
                {
                    string path = str.Replace(pathFrom, pathTo + @"\");
                    if (File.Exists(path))
                    {
                        FileAttributes attributes = File.GetAttributes(path);
                        if ((attributes & FileAttributes.ReadOnly) > 0)
                        {
                            File.SetAttributes(path, attributes & ~FileAttributes.ReadOnly);
                        }
                    }
                    File.Copy(str, path, true);
                }
                foreach (string str3 in Directory.GetDirectories(pathFrom))
                {
                    if (((File.GetAttributes(str3) & FileAttributes.ReparsePoint) != FileAttributes.ReparsePoint) && ((File.GetAttributes(str3) & FileAttributes.Hidden) != FileAttributes.Hidden))
                    {
                        CopyDirectory(str3, str3.Replace(pathFrom, pathTo + @"\"), filter, level + 1);
                    }
                }
            }
        }

        public static void EnsureFileCanWrite(string path)
        {
            FileAttributes fileAttributes = File.GetAttributes(path) & ~FileAttributes.ReadOnly;
            fileAttributes &= ~FileAttributes.Hidden;
            File.SetAttributes(path, fileAttributes);
        }

        public static void EnsureFileCreated(string path)
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
        }

        public static void FindAndReplace(string fileName, string find, string replace)
        {
            string str;
            using (StreamReader reader = new StreamReader(fileName))
            {
                str = reader.ReadToEnd();
                reader.Close();
            }
            str = Regex.Replace(str, find, replace);
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                writer.Write(str);
                writer.Close();
            }
        }

        public static string FindFileInDirectory(string find, string path)
        {
            find = find.ToLower();
            foreach (string str in Directory.GetFiles(path))
            {
                if (str.ToLower().Contains(find))
                {
                    return str;
                }
            }
            return string.Empty;
        }

        public static FileSystemInfo GetFileSystemInfo(string path)
        {
            if (File.Exists(path))
            {
                return new FileInfo(path);
            }
            if (Directory.Exists(path))
            {
                return new DirectoryInfo(path);
            }
            return null;
        }

        public static void GrantFilePermissionAccess(string path, FileIOPermissionAccess permission)
        {
            new FileIOPermission(permission, path).Demand();
        }

        public static string MapPath(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            if (HostingEnvironment.IsHosted)
            {
                return HostingEnvironment.MapPath(path);
            }
            if (!VirtualPathUtility.IsAppRelative(path))
            {
                throw new Exception("Could not resolve non-rooted path.");
            }
            string str = VirtualPathUtility.ToAbsolute(path, "/").Replace('/', '\\').Substring(1);
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, str);
        }

        public static string ModifyConnectionString(string connectionString)
        {
            if (connectionString.Contains("MultipleActiveResultSets"))
            {
                return connectionString;
            }
            string str = connectionString.Trim();
            return (str + (str.EndsWith(";") ? "MultipleActiveResultSets=true;" : ";MultipleActiveResultSets=true;"));
        }

        public static void ReplaceAllFilesInDirectory(string directoryFind, string directoryReplace)
        {
            ReplaceAllFilesInDirectory(directoryFind, directoryReplace, string.Empty);
        }

        public static void ReplaceAllFilesInDirectory(string directoryFind, string directoryReplace, string fileExtension)
        {
            if (fileExtension != string.Empty)
            {
                foreach (string str in Directory.GetFiles(directoryFind, fileExtension))
                {
                    File.Copy(str.Replace(directoryFind, directoryReplace), str, false);
                }
            }
            else
            {
                foreach (string str2 in Directory.GetFiles(directoryFind))
                {
                    File.Copy(str2.Replace(directoryFind, directoryReplace), str2, false);
                }
            }
        }

        public static string ToValidFileName(string s)
        {
            return ToValidFileName(s, string.Empty, null);
        }

        public static string ToValidFileName(string s, string invalidReplacement)
        {
            return ToValidFileName(s, invalidReplacement, null);
        }

        public static string ToValidFileName(string s, string invalidReplacement, string spaceReplacement)
        {
            StringBuilder builder = new StringBuilder(s);
            foreach (char ch in Path.GetInvalidFileNameChars())
            {
                if (invalidReplacement != null)
                {
                    builder.Replace(ch.ToString(), invalidReplacement);
                }
            }
            if (spaceReplacement != null)
            {
                builder.Replace(" ", spaceReplacement);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Get application full path
        /// </summary>
        /// <param name="file"></param>
        /// <returns>full path from application domain base directory (If file is relative path)</returns>
        public static string GetApplicationFullPath(string file)
        {
            if (HttpContext.Current != null && file.StartsWith("~"))
            {
                file = HttpContext.Current.Server.MapPath(file);
            }

            if (!Path.IsPathRooted(file))
            {
                file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
            }
            return Path.GetFullPath(file);
        }

        public static double GetMegaByte(int value)
        {
            return value * 1024 * 1024;
        }

        public static string DisplayFileSize(long fileSize)
        {
            var units = new string[] { "Bytes", "KB", "MB", "GB", "TB" };
            double temp = fileSize;
            foreach (var unit in units)
            {
                if (temp < 1024) { return String.Format("{0:N2} {1}", temp, unit); }
                temp = temp / 1024;
            }
            return String.Format("{0:N2}", fileSize);
        }
    }
}

