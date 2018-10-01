using Newtonsoft.Json;
using SECOM.ACS.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    //public static class BlueImpGalleryHelper
    //{
    //    public static IList<BlueImpGalleryData> LoadFromFile(string file)
    //    {
    //        var dataItems = JsonDataContext.GetDataFromJsonFile<BlueImpGalleryData>(file).Where(t=>!String.IsNullOrEmpty(t.ImageUrl)).ToList();
    //        return dataItems;
    //    }

    //    public static string ToJson(this IList<BlueImpGalleryData> data,HttpContext context)
    //    {
    //        return JsonConvert.SerializeObject(data.Select(t =>
    //        {
    //            var helper = new UrlHelper(context.Request.RequestContext);
    //            return new { title = t.Title, href = helper.Content(t.ImageUrl) };
    //        }));
    //    }
    //}

    public class UniteGalleryData
    {
        [JsonProperty("alt")]
        public string AlternativeText { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailImageUrl { get; set; }

    }
}