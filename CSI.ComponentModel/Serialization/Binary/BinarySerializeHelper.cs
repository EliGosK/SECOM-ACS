namespace CSI.Serialization.Binary
{
    using CSI.IO;
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    public static class BinarySerializeHelper
    {
        public static T Deserialize<T>(string path) where T: class, new()
        {
            if (File.Exists(path))
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x200))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return (T) formatter.Deserialize(stream);
                }
            }
            throw new FileNotFoundException("File to deserialze is not found.", path);
        }

        public static void Serialize<T>(string path, T value) where T: class, new()
        {
            FileInfo file = new FileInfo(path);
            DirectoryHelper.EnsureDirectoryCreated(file);
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 0x200))
            {
                new BinaryFormatter().Serialize(stream, value);
            }
        }

        public static void Serialize(string path, object value)
        {
            FileInfo file = new FileInfo(path);
            DirectoryHelper.EnsureDirectoryCreated(file);
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 0x200))
            {
                new BinaryFormatter().Serialize(stream, value);
            }
        }
    }
}

