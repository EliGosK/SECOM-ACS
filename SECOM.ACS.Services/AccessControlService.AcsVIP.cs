using CSI.ComponentModel;
using CSI.Core;
using SECOM.ACS.Data;
using SECOM.ACS.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Services
{
    public partial class AccessControlService 
    {
        public IEnumerable<VIPCardRegistrationView> GetVIPCardRegistrationViews(VIPCardRegistrationSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                var dataItems = unitOfWork.AcsVIPs.GetVIPCardRegistrationViews(criteria);
                foreach (var dataItem in dataItems)
                {
                    LoadData(unitOfWork, dataItem);
                }
                return dataItems;
            }
        }

        private void LoadData(IUnitOfWork u, VIPCardRegistrationView dataItem)
        {
            if (dataItem.PositionMiscID.HasValue)
            {
                dataItem.PositionType = u.Miscs.Find(t => t.MiscType == MiscTypes.VIPPosition && t.MiscID == dataItem.PositionMiscID.Value).FirstOrDefault();
            }

            if (!String.IsNullOrEmpty(dataItem.AreaID))
            {
                var areas = new List<AreaDataView>();
                foreach (var area in dataItem.AreaID.Split(','))
                {
                    int areaId = 0;
                    if (Int32.TryParse(area,out areaId))
                    {
                        var findArea = u.Areas.GetDataView(areaId);
                        if (findArea != null) { areas.Add(findArea); }
                    }                   
                }
                dataItem.Area = areas.ToArray();
            }
        }

        public ObjectResult CreateAcsVIP(AcsVIP entity)
        {
            bool acsInserted = false;
            using (var u = CreateUnitOfWork())
            {
                try
                {
                    //Receive Card Status
                    entity.Status = CardVIPStatus.Unavailable;
                    u.AcsVIPs.Add(entity);

                    acsInserted = true;
                    // Insert Req Area Mapping
                    foreach (var area in entity.ReqAreaMapping)
                    {
                        area.ReqNo = entity.ReqNo;
                        u.ReqAreaMappings.Add(area);
                    }

                    // Insert Transaction
                    var trans = new TransactionAcs()
                    {
                        TranID = Guid.NewGuid(),
                        ReqNo = entity.ReqNo,
                        DetailID = Guid.Empty,
                        EntryDateFrom = DateTime.Now.Date,
                        EntryTimeFrom = TimeSpan.FromHours(8),
                        EntryDateTo = DateTime.Now.Date,
                        EntryTimeTo = TimeSpan.FromHours(20),
                        CardID = entity.CardID,
                        CardReceiveTime = DateTime.Now.Date,
                        Status = (int)TransactionStatus.SendCardToACS,
                        UpdateBy = entity.CreateBy,
                        UpdateDate = DateTime.Now
                    };
                    u.AcsTransactions.Add(trans);
                    u.Complete();
                    entity.TransactionAcs.Add(trans);
                    return ObjectResult.Succeed(entity);
                }
                catch (Exception ex)
                {
                    if (acsInserted)
                    {
                        u.AcsVIPs.Remove(entity);
                        u.ReqApprovers.RemovesByRequestNo(entity.ReqNo);
                        u.Complete();
                    }
                    return ObjectResult.Fail(ex);
                }
            }

        }

        public ObjectResult UpdateAcsVIP(AcsVIP entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    u.AcsVIPs.Edit(entity);
                    var transactionToUpdated = u.AcsTransactions.Find(t => t.ReqNo == entity.ReqNo).ToList();
                    foreach (var t in transactionToUpdated)
                    {
                        t.EntryDateTo = DateTime.Now;
                        t.EntryTimeTo = DateTime.Now.TimeOfDay;
                        t.CardID = entity.CardID;
                        t.CardReturnTime = DateTime.Now;
                        t.UpdateBy = entity.UpdateBy;
                        t.UpdateDate = DateTime.Now;
                    }                           
                    u.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult DeleteAcsVIP(AcsVIP entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    u.ReqAreaMappings.RemovesByRequestNo(entity.ReqNo);
                    u.AcsTransactions.Find(t => t.ReqNo == entity.ReqNo).ToList().ForEach((TransactionAcs t) => u.AcsTransactions.Remove(t));
                    u.AcsVIPs.Remove(entity);
                    u.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }

        }
    }
}
