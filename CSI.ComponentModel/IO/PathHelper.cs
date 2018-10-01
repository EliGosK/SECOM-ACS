using System;
using System.IO;
using System.Web;

namespace CSI.IO
{
    public class PathHelper
    {
        public static string GetPhysicalFullPath(string path)
        {
            if (File.Exists(path))
            {
                return path;
            }
            if (Path.IsPathRooted(path))
            {
                return path;
            }
            if (HttpContext.Current == null)
            {
                return Path.GetFullPath(path);
            }
            if (VirtualPathUtility.IsAbsolute(path))
            {
                return path;
            }
            return HttpContext.Current.Server.MapPath(path);
        }

        public static string GetRelativePath(string path, string basePath)
        {
            return GetRelativePath(path, basePath, true);
        }

        public static string GetRelativePath(string path, string basePath, bool convertToUNC)
        {
            if (IsDirectory(path))
            {
                path = path.TrimEnd(new char[] { Path.DirectorySeparatorChar }) + Path.DirectorySeparatorChar;
            }
            if (IsDirectory(basePath))
            {
                basePath = basePath.TrimEnd(new char[] { Path.DirectorySeparatorChar }) + Path.DirectorySeparatorChar;
            }
            Uri uri = new Uri(Path.GetFullPath(path), UriKind.Absolute);
            Uri uri2 = new Uri(Path.GetFullPath(basePath), UriKind.Absolute);
            string str = uri.MakeRelativeUri(uri2).ToString();
            if (convertToUNC)
            {
                return str.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }
            return str;
        }

        public static bool IsDirectory(string path)
        {
            if (File.Exists(path))
            {
                return ((File.GetAttributes(path) & FileAttributes.Directory) > 0);
            }
            return true;
        }
    }
}

