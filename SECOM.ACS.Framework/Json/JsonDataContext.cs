using CSI.Web.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.Json
{
    public static class JsonDataContext
    {
        public static IEnumerable<TData> GetDataFromJsonFile<TData>(string file)
        {
            file = PathUtility.GetPhysicalPath(file);
            if (!System.IO.File.Exists(file))
            {
                return new List<TData>();
            }

            using (var sr = System.IO.File.OpenText(file))
            {
                JsonSerializer serializer = new JsonSerializer();
                return (List<TData>)serializer.Deserialize(sr, typeof(List<TData>));
            }
        }
    }
}