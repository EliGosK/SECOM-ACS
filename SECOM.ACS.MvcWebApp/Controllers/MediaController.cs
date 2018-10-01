using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    public class MediaController : AppControllerBase
    {
        public FileResult Download(string id, string filename)
        {
            var fs = TempData[id] as MemoryStream;
            return File(fs, MimeMapping.GetMimeMapping(filename), filename);
        }
    }
}