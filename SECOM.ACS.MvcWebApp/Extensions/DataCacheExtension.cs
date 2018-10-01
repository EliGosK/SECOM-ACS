using SECOM.ACS.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SECOM.ACS.MvcWebApp.Extensions
{
    public static class DataCacheExtension
    {
        public static string GetDocumentName(this DataCacheContext dataContext, string documentType)
        {
            var dataItem = ApplicationContext.DataContext.SystemMiscs.Where(t => t.SysMiscType == "DocType" && String.Compare(t.SysMiscCode, documentType, true) == 0).FirstOrDefault();
            if (dataItem != null) {
                return dataItem.SysMiscValue1;
            }
            return documentType;
        }
    }
}