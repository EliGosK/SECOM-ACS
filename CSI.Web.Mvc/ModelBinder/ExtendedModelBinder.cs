using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CSI.Web.Mvc
{
    public class ExtendedModelBinder : DefaultModelBinder
    {
        protected override void BindProperty(ControllerContext controllerContext,ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            var propBindAttr = propertyDescriptor.Attributes.OfType<PropertyBinderAttribute>().FirstOrDefault();

            if (propBindAttr != null && propBindAttr.BindProperty(controllerContext, bindingContext, propertyDescriptor))
            {
                return;
            }
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }

   
}
