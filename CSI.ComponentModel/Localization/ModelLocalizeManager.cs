using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSI.Localization
{
    /// <summary>
    /// 
    /// </summary>
    public class ModelLocalizeManager
    {
        protected TValue GetValue<TValue>(string culture,TValue value, IDictionary<string, TValue> fields)
        {
            var v = fields.Keys.Where(t => String.Compare(t, culture, true) == 0).FirstOrDefault();
            if (v == null) { return value; }
            return fields[v];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="name">Localize property group name</param>
        /// <returns></returns>
        public static string GetValue<TModel>(TModel model, string name)
        {
            return GetValue(model, name, System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="name">Localize property group name</param>
        /// <param name="culture">Culture name. Ex. th-TH, en-US</param>
        /// <returns></returns>
        public static string GetValue<TModel>(TModel model,string name, string culture)
        {
            var attrs = typeof(TModel).GetCustomAttributes(typeof(LocalizePropertyAttribute), true) as LocalizePropertyAttribute[];
            if (attrs == null) { return null; }
            var findAttrs = attrs.Where(t => t.Name == name).ToList();
            if (findAttrs.Count == 0) { return null; }

            var propertyName = string.Empty;
            foreach (var attr in findAttrs)
            {
                if (attr.Culture == culture) { propertyName = attr.PropertyName; break; }
            }

            if (String.IsNullOrEmpty(propertyName))
            {
                var v = findAttrs.Where(t => String.IsNullOrEmpty(t.Culture)).FirstOrDefault();
                if (v == null) { return null; }
                propertyName = v.PropertyName;
            }

            var p = typeof(TModel).GetProperty(propertyName);
            if (p == null) { return null; }
            return (string)p.GetValue(model, null);
        }

        public static string GetPropertyName<TModel>(string name)
           where TModel : class
        {
            return GetPropertyName<TModel>(name, System.Threading.Thread.CurrentThread.CurrentUICulture);
        }

        public static string GetPropertyName<TModel>(string name,CultureInfo culture) 
            where TModel : class
        {
            var attrs = typeof(TModel).GetCustomAttributes(typeof(LocalizePropertyAttribute), true) as LocalizePropertyAttribute[];

            var dataItem = attrs.FirstOrDefault(t => t.Name == name && t.Culture == culture.TwoLetterISOLanguageName);
            if (dataItem == null) {
                dataItem = attrs.FirstOrDefault(t => t.Name == name && String.IsNullOrEmpty(t.Culture));
            }
            return dataItem.PropertyName;
        }
    }
}
