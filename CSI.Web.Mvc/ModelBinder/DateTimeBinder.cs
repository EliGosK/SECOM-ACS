using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CSI.Web.Mvc
{
    public class DateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            return GetValue(value);
        }

        private object GetValue(ValueProviderResult result)
        {
            try
            {
                return result.ConvertTo(typeof(DateTime), result.Culture);
            }
            catch
            {
                // Sat Nov 04 2017 00:00:00 GMT+0700 (SE Asia Standard Time) => Nov 04 2017 00:00:00
                var dateString = result.AttemptedValue.Substring(4, 20);
                DateTime dateResult;
                if (DateTime.TryParseExact(dateString, "MMM dd yyyy HH:mm:ss", result.Culture, DateTimeStyles.None, out dateResult))
                {
                    return dateResult;
                }
                return DateTime.MinValue;
            }
        }
    }

    public class NullableDateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, value);

            return value == null
                ? null
                : GetValue(value);
        }

        private object GetValue(ValueProviderResult result)
        {
            try
            {
                return result.ConvertTo(typeof(DateTime), result.Culture);
            }
            catch
            {
                // Sat Nov 04 2017 00:00:00 GMT+0700 (SE Asia Standard Time) => Nov 04 2017 00:00:00
                var dateString = result.AttemptedValue.Substring(4, 20);
                DateTime dateResult;
                if (DateTime.TryParseExact(dateString, "MMM dd yyyy HH:mm:ss", result.Culture, DateTimeStyles.None, out dateResult))
                {
                    return dateResult;
                }
                return null;
            }
        }
    }
}
