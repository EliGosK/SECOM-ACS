using CSI.Text;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.Json;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.Reporting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    public class TestController : AppControllerBase
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Sample1()
        {
            return View();
        }

        public ActionResult Export()
        {
            return View();
        }

        public ActionResult ErrorSample()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ExportSampleAsync(RequestSearchCriteria criteria)
        {
            try
            {
                var dataItems = GetSampleData();
                var reportBuilder = new SampleReportBuilder();
                var ms = new MemoryStream();
                var reportData = new SampleReportData()
                {
                    Data = dataItems,
                    HeaderNames = AcsModelHelper.GetModelDisplayNames<RequestDataView>()
                };


                reportBuilder.CreateReport(ms, reportData);
                ms.Position = 0;
                string fileDownloadName = "request-inquiry.xlsx";
                var fileKey = TextGenerator.Generate(32).ToLowerInvariant();
                TempData[fileKey] = ms;
                return JsonNet(new { id = fileKey, filename = fileDownloadName, message = MessageHelper.GenerateReportSuccess("Request Inquiry") }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private IList<SampleData> GetSampleData()
        {
            var dataItem1 = new SampleData()
            {
                ReqNo = "Request 1",
                AreaEN = "Area En 1",
                ObjectID = "ACS050"
            };

            var dataItem2 = new SampleData()
            {
                ReqNo = "Request 2",
                AreaEN = "Area En 2",
                ObjectID = "ACS050"
            };

            var dataItem3 = new SampleData()
            {
                ReqNo = "Request 3",
                AreaEN = "Area En 3",
                ObjectID = "ACS050"
            };

            return new List<SampleData>() { dataItem1, dataItem2, dataItem3 };

        }

        [HttpPost]
        public ActionResult Error()
        {
            return InternalServerError(new Exception("This is error message."));
        }
    }
}