using CSI.IO;
using Newtonsoft.Json;
using SECOM.ACS.MvcWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace SECOM.ACS.MvcWebApp.Helper
{
    public class OfflineSystemFileManager
    {
        public static OfflineOnlineSystemData LoadFromFile(string file)
        {
            if (file.StartsWith("~"))
                file = HostingEnvironment.MapPath(file);

            if (!File.Exists(file)) { return null; }

            try
            {
                using (StreamReader sr = File.OpenText(file))
                {
                    JsonReader r = new JsonTextReader(sr);
                    JsonSerializer serializer = new JsonSerializer();
                    var data = serializer.Deserialize<OfflineOnlineSystemData>(r);
                    data.Calculate();
                    return data;
                }
            }
            catch 
            {
                return null;
            }
            
        }

        public static void WriteFile(string file, OfflineOnlineSystemData data)
        {
            if (file.StartsWith("~"))
                file = HostingEnvironment.MapPath(file);

            DirectoryHelper.EnsureDirectoryCreated(file);
            using (StreamWriter fs = File.CreateText(file))
            {
                JsonTextWriter writer = new JsonTextWriter(fs);
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, data);
            }
        }
    }
}