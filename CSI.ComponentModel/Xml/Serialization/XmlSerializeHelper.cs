namespace CSI.Xml.Serialization
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;

    public static class XmlSerializeHelper
    {
        public static T Deserialize<T>(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T) serializer.Deserialize(stream);
        }

        public static T Deserialize<T>(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 0x200))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        return (T) serializer.Deserialize(stream);
                    }
                }
                catch (Exception exception)
                {
                    throw exception;
                }
            }
            throw new FileNotFoundException($"Could not deserialize from xml file. File {path} not found.",path);
        }

        public static T Deserialize<T>(XmlReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T) serializer.Deserialize(reader);
        }

        public static T Deserialize<T>(XmlReader reader, string encodingStyle)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T) serializer.Deserialize(reader, encodingStyle);
        }

        public static void Serialize<T>(Stream stream, T value)
        {
            new XmlSerializer(typeof(T)).Serialize(stream, value);
        }

        public static void Serialize<T>(TextWriter writer, T value)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, value);
        }

        public static void Serialize<T>(string path, T value)
        {
            FileInfo info = new FileInfo(path);
            if (!info.Directory.Exists)
            {
                info.Directory.Create();
            }
            bool flag = false;
            using (FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 0x400))
            {
                try
                {
                    new XmlSerializer(typeof(T)).Serialize((Stream) stream, value);
                }
                catch (Exception exception)
                {
                    flag = true;
                    throw exception;
                }
                finally
                {
                    if (flag)
                    {
                        info.Delete();
                    }
                }
            }
        }

        public static void Serialize<T>(XmlWriter writer, T value)
        {
            new XmlSerializer(typeof(T)).Serialize(writer, value);
        }
    }
}

