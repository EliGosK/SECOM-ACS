using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSI.IO
{
    public static class DirectoryHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        public static void EnsureDirectoryCreated(FileInfo file)
        {
            EnsureDirectoryCreated(file.Directory.FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static void EnsureDirectoryCreated(string path)
        {
            string str = Path.HasExtension(path) ? Path.GetDirectoryName(path) : path;
            if (!string.IsNullOrEmpty(str) && !Directory.Exists(str))
            {
                Directory.CreateDirectory(str);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folder">Directory to get file</param>
        /// <param name="filters">Array of ext file Ex. jpg, txt, gif</param>
        /// <param name="isRecursive"></param>
        /// <returns></returns>
        public static string[] GetFiles(string folder,string[] filters,bool isRecursive)
        {
            var filesFound = new List<String>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                filesFound.AddRange(Directory.GetFiles(folder, String.Format("*.{0}", filter), searchOption));
            }
            return filesFound.ToArray();
        }

        public static IEnumerable<FileInfo> GetFileInfoInDirectory(string folder, string[] filters, bool isRecursive)
        {
            var filesFound = new List<FileInfo>();
            var searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var filter in filters)
            {
                var files = Directory.GetFiles(folder, String.Format("*.{0}", filter), searchOption);
                foreach (var file in files)
                {
                    filesFound.Add(new FileInfo(file));
                }
            }
            return filesFound;
        }
    }
}

