using CSI.ComponentModel;
using CSI.Exceptions;
using CSI.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using SECOM.ACS.Models;
using SECOM.ACS.MvcWebApp.Extensions;
using SECOM.ACS.MvcWebApp.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Web.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Controllers
{
    [ApplicationAuthorize]
    public class CardController : AppControllerBase
    {
         private readonly IAccessControlService service;

        public CardController(IAccessControlService service)
        {
            this.service = service;
        }

        [ApplicationAuthorize("MAS060", PermissionNames.View)]
        [SiteMapPageTitle("MAS060")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [NoCache]
        public ActionResult List([DataSourceRequest]DataSourceRequest request, CardSearchCriteria criteria)
        {
            criteria.EnsureDataValid();
            var dataItems = service.GetCardsByCriteria(criteria);
            var result = dataItems.ToDataSourceResult(request, (Card card) => card.ToViewModel());
            return JsonNet(result, JsonRequestBehavior.AllowGet);
        }

        [ApplicationSuspend]
        [IgnoreModelErrors("CreateDate, UpdateDate")]
        public ActionResult Delete(CardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var item = service.GetCard(model.CardID);
            if (item == null)
            {
                return InternalServerError("Delete failed. Card data not found or delete by other user.");
            }
            var entity = model.ToEntity();
            entity.UpdateBy = User.Identity.Name;
            var result = service.DeleteCard(entity);
            if (result.IsSucceed)
                return Ok(MessageHelper.DeleteCompleted());
            else
                return InternalServerError(MessageHelper.DeleteFailed(result.GetErrorMessage()));
        }

        [ApplicationSuspend]
        [IgnoreModelErrors("CreateDate, UpdateDate")]
        public ActionResult Update(CardViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var entity = model.ToEntity();
            entity.UpdateBy = User.Identity.Name;
            entity.CreateBy = User.Identity.Name;
            var result = service.UpdateCard(entity);
            if (result.IsSucceed)
                return Ok((MessageHelper.SaveCompleted()));
            else
                return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
            
        }

        [ApplicationSuspend]
        [IgnoreModelErrors("CreateDate, UpdateDate")]
        public ActionResult Create(CardViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var entity = viewModel.ToEntity();
                entity.CreateBy = User.Identity.Name;
                entity.UpdateBy = User.Identity.Name;
                var result = service.CreateCard(entity);
                if (result.IsSucceed)
                {

                    return Ok(MessageHelper.SaveCompleted());
                }
                else
                {
                    if (result.Error.GetType() == typeof(DuplicateDataException))
                    {
                        var args = new string[]{
                            ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(CardViewModel), "CardType").GetDisplayName(),
                            ModelMetadataProviders.Current.GetMetadataForProperty(null, typeof(CardViewModel), "CardNo").GetDisplayName(),
                            entity.CardType,
                            entity.CardNo
                        };
                        return InternalServerError(MessageHelper.DuplicateField(args));
                    }
                    return InternalServerError(MessageHelper.SaveFailed(result.GetErrorMessage()));
                }
            }
            return BadRequest();
        }

        [ApplicationSuspend]
        public ActionResult Upload(IEnumerable<HttpPostedFileBase> files)
        {
            var dataItems = new List<CardImportData>();
            var errors = new List<ImportDataError>();
            foreach (var f in files)
            {
                using (var fs = new StreamReader(f.InputStream))
                {
                    using (var reader = new CsvHelper.CsvReader(fs))
                    {
                        reader.Configuration.RegisterClassMap<CardImportDataMap>();
                        reader.Configuration.HasHeaderRecord = true;
                        reader.Configuration.Delimiter = "|";
                        while (reader.Read())
                        {
                            try
                            {
                                var dataItem = reader.GetRecord<CardImportData>();
                                dataItems.Add(dataItem);
                            }
                            catch (Exception ex)
                            {
                                errors.Add(new ImportDataError() { Data = reader.CurrentRecord, Error = ExceptionUtility.GetLastExceptionMessage(ex) });
                            }
                            
                        }
                    }
                }
            }
            return JsonNet(new { Data = dataItems, Errors = errors }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ApplicationSuspend]
        [ValidateAntiForgeryToken]
        public ActionResult Import(IList<CardImportData> data)
        {
            var cards = new List<Card>();
            data.Each((CardImportData c) => cards.Add(c.ToEntity(User.Identity.Name)));
            var results = service.ImportCard(cards);
            return JsonNet(results, JsonRequestBehavior.AllowGet);
            
        }
    }
}