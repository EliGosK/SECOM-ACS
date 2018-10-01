using CSI.ComponentModel;
using CSI.Core;
using SECOM.ACS.Data;
using SECOM.ACS.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace SECOM.ACS.Services
{
    public partial class DocumentService : IDocumentService
    {
        protected IUnitOfWork CreateUnitOfWork()
        {
            var context = new ACSContext();
            context.Configuration.ProxyCreationEnabled = false;
            return new UnitOfWork(context);
        }

        public UpdateDocumentResult UpdateDocumentStatus(string user,int documentTypes)
        {
            using (var u = CreateUnitOfWork())
            {
                var requestToSetExpires = new List<IAcsRequest>();
                var requestToSetDones = new List<IAcsRequest>();

                try
                {
                    var updateDate = DateTime.MinValue;
                    if (!u.AcsTasks.UpdateDocument(documentTypes,out updateDate))
                    {
                        return new UpdateDocumentResult(new UpdateDocumentExpirationData());
                    }

                    var acsEmployees = u.AcsEmployees.Find(t => t.UpdateDate == updateDate).ToList();
                    acsEmployees.Where(t => t.Status == RequestStatus.Expired).ToList().ForEach((AcsEmployee dataItem) => requestToSetExpires.Add(dataItem));
                    acsEmployees.Where(t => t.Status == RequestStatus.Done).ToList().ForEach((AcsEmployee dataItem) => requestToSetDones.Add(dataItem));

                    var acsVisitors = u.AcsVisitors.Find(t => t.UpdateDate == updateDate);
                    acsVisitors.Where(t => t.Status == RequestStatus.Expired).ToList().ForEach((AcsVisitor dataItem) => requestToSetExpires.Add(dataItem));
                    acsVisitors.Where(t => t.Status == RequestStatus.Done).ToList().ForEach((AcsVisitor dataItem) => requestToSetDones.Add(dataItem));

                    var acsItemIns = u.AcsItemIns.Find(t => t.UpdateDate == updateDate);
                    acsItemIns.Where(t => t.Status == RequestStatus.Expired).ToList().ForEach((AcsItemIn dataItem) => requestToSetExpires.Add(dataItem));
                    acsItemIns.Where(t => t.Status == RequestStatus.Done).ToList().ForEach((AcsItemIn dataItem) => requestToSetDones.Add(dataItem));

                    var acsItemOuts = u.AcsItemOuts.Find(t => t.UpdateDate == updateDate);
                    acsItemOuts.Where(t => t.Status == RequestStatus.Expired).ToList().ForEach((AcsItemOut dataItem) => requestToSetExpires.Add(dataItem));
                    acsItemOuts.Where(t => t.Status == RequestStatus.Done).ToList().ForEach((AcsItemOut dataItem) => requestToSetDones.Add(dataItem));

                    var acsPhotoes = u.AcsPhotos.Find(t => t.UpdateDate == updateDate);
                    acsPhotoes.Where(t => t.Status == RequestStatus.Expired).ToList().ForEach((AcsPhoto dataItem) => requestToSetExpires.Add(dataItem));
                    acsPhotoes.Where(t => t.Status == RequestStatus.Done).ToList().ForEach((AcsPhoto dataItem) => requestToSetDones.Add(dataItem));

                    return new UpdateDocumentResult(new UpdateDocumentExpirationData(requestToSetExpires.ToArray(), requestToSetDones.ToArray()));
                }
                catch (Exception ex)
                {
                    return new UpdateDocumentResult(new UpdateDocumentExpirationData(), ex);
                }
            }
        }
    }
}
