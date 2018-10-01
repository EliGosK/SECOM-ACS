using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Mvc;

namespace CSI.Web.Mvc
{
    public static class ModelMetadataUtility
    {
        public static Dictionary<string, string> GetModelDisplayNames(Type type)
        {
            var values = new Dictionary<string, string>();
            var properties = type.GetProperties();
            foreach (var p in properties)
            {
                var metaData = ModelMetadataProviders.Current.GetMetadataForProperty(null, type, p.Name);
                string value = metaData.DisplayName ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(p.Name));
                values.Add(p.Name, value);
            }
            return values;
        }

        public static Dictionary<string, string> GetModelDisplayNames<TModel>() where TModel : class
        {
            var values = new Dictionary<string, string>();
            var properties = typeof(TModel).GetProperties();
            foreach (var p in properties)
            {
                var metaData = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(TModel), p.Name);
                string value = metaData.DisplayName ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(p.Name));
                values.Add(p.Name, value);
            }
            return values;
        }

        public static string GetModelDisplayName<TModel>(string propertyName) where TModel : class
        {
          
            var metaData = ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(TModel), propertyName);
            return metaData.DisplayName ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(propertyName));
        }

        public static string GetModelDisplayName(Type modelType, string propertyName) 
        {

            var metaData = ModelMetadataProviders.Current.GetMetadataForProperty(null, modelType, propertyName);
            return metaData.DisplayName ?? (metaData.PropertyName ?? ExpressionHelper.GetExpressionText(propertyName));
        }

    }

    public static class PathUtility
    {
        public static string GetPhysicalPath(string virtualPath)
        {
            if (virtualPath.Contains("~"))
                return HostingEnvironment.MapPath(virtualPath);
            return virtualPath;
        }
    }
}
