using CSI.ComponentModel;
using CSI.Core;
using SECOM.ACS.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using CSI.Linq;

namespace SECOM.ACS.Services
{
    public partial class AccessControlService : IMasterService
    {
        #region Area
        public Area GetArea(int id)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Areas.Get(id);
            }
        }

        public IEnumerable<Position> GetPositionsForAreaApprover()
        {
            using (var unitofWork = CreateUnitOfWork())
            {
                return unitofWork.AreaApprovers.GetPositions().ToList();
            }
        }

        public IEnumerable<Department> GetDepartmentForAreaApprover()
        {
            using (var unitofWork = CreateUnitOfWork())
            {
                return unitofWork.AreaApprovers.GetDepartments().ToList();
            }
        }
        public IEnumerable<AreaMapping> GetAreaMapping(string employeeId)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Areas.GetAreaMapping(employeeId);
            }
        }

        public IEnumerable<Area> GetAllArea()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Areas.GetAll();
            }
        }

        public IEnumerable<Area> FindArea(AreaSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Areas.Find(criteria);
            }
        }

        public IEnumerable<Area> GetAreaByCriteria(AreaSearchCriteria criteria, AreaLoadOptions loadOptions)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                var dataItems = unitOfWork.Areas.GetBySearchCriteria(criteria);
                foreach (var dataItem in dataItems)
                {
                    LoadData(unitOfWork, dataItem, loadOptions);
                    LoadApproverData(unitOfWork, dataItem);
                }
                return dataItems;
            }
        }

        /// <summary>
        /// Load relate data for area
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="area"></param>
        /// <param name="loadOptions"></param>
        private void LoadData(IUnitOfWork unitOfWork, Area area, AreaLoadOptions loadOptions)
        {
            if ((loadOptions & AreaLoadOptions.Gate) > 0)
            {
                unitOfWork.Areas.LoadGate(area);
            }
        }

        private void LoadApproverData(IUnitOfWork unitOfWork, Area area)
        {
            unitOfWork.Areas.LoadAreaApprover(area);
        }

        public IEnumerable<AreaDataView> GetAreaDataViews(string user,bool areaMapping)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Areas.GetDataViews(user,areaMapping);
            }
        }

        public ObjectResult CreateArea(Area area)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    if (unitOfWork.Areas.IsDuplicate(area))
                    {
                        throw new DuplicateDataException();
                    }
                    unitOfWork.Areas.Add(area);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed(area);
                }
            }
            catch (Exception ex) 
            {
                return ObjectResult.Fail(ex);
            }
           
        }

        public ObjectResult DeleteArea(Area area)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    if (unitOfWork.Areas.IsInUse(area.AreaID))
                    {
                        throw new Exception("Data is in use");
                    }

                    unitOfWork.Areas.Remove(area);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed(area);
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
            
        }

        public ObjectResult UpdateArea(Area area)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    var findArea = unitOfWork.Areas.SingleOrDefault(t => t.AreaID == area.AreaID);
                    if (findArea == null)
                    {
                        throw new Exception("Area data not found");
                    }

                    unitOfWork.Areas.Edit(area);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed(area);
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
           
        }
        

        #endregion

        #region Card
        public Card GetCard(string id)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Cards.GetCardForCheckData(id).FirstOrDefault();
            }
        }
        public IEnumerable<Card> GetCardsByCriteria(CardSearchCriteria criteria)
        {
            criteria.EnsureDataValid();
            using (var unitOfWork = CreateUnitOfWork())
            {
                var dataItems = unitOfWork.Cards.GetBySearchCriteria(criteria)
                    .ToList();
                foreach (var card in dataItems)
                {
                    LoadData(card);
                }
                return dataItems;
            }
        }

        private void LoadData(Card card)
        {
            foreach (var mapping in card.AreaCardMappings)
            {
                mapping.Area = GetArea(mapping.AreaID);
            }
        }

        public IEnumerable<Card> GetAllCard()
        {
            throw new NotImplementedException();
        }

        public ObjectResult CreateCard(Card card)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    if (unitOfWork.Cards.IsDuplicate(card))
                    {
                        throw new DuplicateDataException();
                    }
                    unitOfWork.Cards.Add(card);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed(card);
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResults<ImportCardResult> ImportCard(IEnumerable<Card> cards)
        {
            var results = new ObjectResults<ImportCardResult>();
            using (var unitOfWork = CreateUnitOfWork())
            {
                foreach (var card in cards)
                {
                    try
                    {
                        unitOfWork.Cards.AddOrUpdate(card);
                        results.AddResult(new ImportCardResult(card));
                    }
                    catch (Exception ex)
                    {
                        results.AddResult(new ImportCardResult(card, ex));
                    }
                }
            }
            return results;
        }

        public ObjectResult DeleteCard(Card entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    if (unitOfWork.Cards.IsInUse(entity.CardID))
                    {
                        throw new Exception($"Delete card error. Card ID {entity.CardID} is in use.");
                    }

                    unitOfWork.Cards.Remove(entity);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult UpdateCard(Card entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    var findCard = unitOfWork.Cards.Get(entity.CardID);
                    if (findCard == null)
                    {
                        throw new Exception($"Card ID {entity.CardID} data not found");
                    }
                    unitOfWork.Cards.Edit(entity);
                    var cardMappingToAdds = entity.AreaCardMappings.ToList();
                    var cardMappings = unitOfWork.AreaCardMappings.Find(t => t.CardID == entity.CardID).ToList();

                    var qToRemove = from update in cardMappings
                                        join data in entity.AreaCardMappings
                                        on update.AreaID equals data.AreaID
                                        into temp
                                        from data in temp.DefaultIfEmpty(new AreaCardMapping{ CardID = update.CardID, AreaID = 0 })
                                        select new
                                        {
                                            CardID = update.CardID,
                                            AreaID = update.AreaID,
                                            AreaID2 = data.AreaID
                                        };

                    foreach (var item in qToRemove.Where(t=>t.AreaID2 ==0))
                    {
                        var cardToRemove = cardMappings.Find(t => t.AreaID == item.AreaID);
                        if (cardToRemove == null) { continue; }
                        unitOfWork.AreaCardMappings.Remove(cardToRemove);
                    }

                    var qToAdd = from data in entity.AreaCardMappings
                                    join update in cardMappings
                                    on data.AreaID equals update.AreaID
                                    into temp
                                    from update in temp.DefaultIfEmpty(new AreaCardMapping { CardID = data.CardID, AreaID = 0 })
                                    select new
                                    {
                                        CardID = data.CardID,
                                        AreaID = data.AreaID,
                                        AreaID2 = update.AreaID
                                    };
                    foreach (var item in qToAdd.Where(t=>t.AreaID2==0))
                    {
                        var cardToAdd = entity.AreaCardMappings.FirstOrDefault(t => t.AreaID == item.AreaID);
                        if (cardToAdd == null) { continue; }
                        unitOfWork.AreaCardMappings.Add(cardToAdd);
                    }
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public IEnumerable<VisitorCardRegistrationView> GetVisitorCardRegistrationsByCriteria(VisitorCardRegistrationSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                var dataItems = unitOfWork.Cards.GetVisitorCardRegistrationViewsByCriteria(criteria);
                foreach (var item in dataItems)
                {
                    LoadData(unitOfWork, item);
                }
                return dataItems;
            }
        }

        public ObjectResult UpdateVisitorCardRegistrationView(VisitorCardRegistrationView entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    if (entity.TranID == null)
                    {
                        throw new Exception("Invalid VisitorCardView. Transaction ID is null");
                    }
                    unitOfWork.Cards.UpdateVisitorCardRegistrationView(entity);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        private void LoadData(IUnitOfWork u, VisitorCardRegistrationView dataItem)
        {
            if (dataItem.VerifyTypeID.HasValue && dataItem.VerifyTypeID.Value > 0)
            {
                dataItem.VerifyType = u.Miscs.Find(t => t.MiscType == MiscTypes.VerifyType && t.MiscID == dataItem.VerifyTypeID.Value).FirstOrDefault();
            }

            if (dataItem.CardID != null)
            {
                dataItem.Card = u.Cards.Find(t => t.CardID == dataItem.CardID).FirstOrDefault();
            }
        }

        public IEnumerable<BusinessTripCardRegistrationView> GetBusinessTripCardRegistrationsByCriteria(BusinessTripCardRegistrationSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                var dataItems = unitOfWork.Cards.GetBusinessTripCardRegistrationViewsByCriteria(criteria);
                foreach (var item in dataItems)
                {
                    LoadData(unitOfWork, item);
                }
                return dataItems;
            }
        }

        public ObjectResult UpdateBusinessTripCardRegistrationView(BusinessTripCardRegistrationView entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {                    
                    unitOfWork.Cards.UpdateBusinessTripCardRegistrationView(entity);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResults<CheckOverlapPeriodCardNoList> CheckOverlapCardRegistration(BusinessTripCardRegistrationView entity)
        {
            var results = new ObjectResults<CheckOverlapPeriodCardNoList>();
            using (var unitOfWork = CreateUnitOfWork())
            {
                var dataItems = unitOfWork.Cards.CheckOverlapPeriodCardNo(entity.EntryDateFrom,entity.EntryDateTo,entity.Card.CardNo).ToList();
                foreach (var dataItem in dataItems)
                {
                    results.AddResult(dataItem, new Exception("Overlap Period Business Trip Card No."));
                }
                return results;
            }
        }

        private void LoadData(IUnitOfWork u, BusinessTripCardRegistrationView dataItem)
        {
            if (dataItem.CardID != null)
            {
                dataItem.Card = u.Cards.Find(t => t.CardID == dataItem.CardID).FirstOrDefault();
            }
        }

        public ObjectResult UpdateVisitorCard(ReceiveReturnVisitorCardDataView dataView)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                var findItem = unitOfWork.AcsVisitorDetails.Get(dataView.DetailID);
                if (findItem == null) {
                    return ObjectResult.Fail(new Exception($"AcsVisitorDetail ID {dataView.DetailID} data not found."));
                }

                findItem.VerifyTypeID = dataView.VerifyTypeID;
                findItem.VerifyNo = dataView.VerifyNo;
                
                unitOfWork.AcsVisitorDetails.Edit(findItem);
                unitOfWork.Complete();
                return ObjectResult.Succeed();
            }
        }

        public IEnumerable<CardNoView> GetCardNoList(RegisterCardSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Cards.GetCardNoList(criteria).ToList().Take(criteria.PageSize);
            }
        }

        public ObjectResult CheckOverlapCardNo(DateTime entryDate, String cardNo)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                var dataItem = unitOfWork.Cards.CheckOverlapCardNo(entryDate, cardNo).FirstOrDefault();
                if (dataItem != null)
                {
                    return ObjectResult.Fail(new OverlapVisitorCardNoException("Overlap Visitor Card No.", dataItem));
                }
                return ObjectResult.Succeed();
            }
        }

        public ObjectResult CheckOverlapPeriodCardNo(DateTime entryDateFrom, DateTime entryDateTo, string cardNo)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                var dataItem = unitOfWork.Cards.CheckOverlapPeriodCardNo(entryDateFrom, entryDateTo, cardNo).FirstOrDefault();
                if (dataItem!=null) {
                    return ObjectResult.Fail(new OverlapPeriodVisitorCardNoException($"Overlap Period Business Trip Card No. {cardNo}", dataItem));
                }
                return ObjectResult.Succeed();
            }
        }
        
        private void LoadData(IUnitOfWork u, ReceiveReturnVisitorCardDataView dataItem)
        {
            if (dataItem.VerifyTypeID.HasValue)
            {
                dataItem.VerifyType = u.Miscs.Find(t => t.MiscType == MiscTypes.VerifyType && t.MiscID == dataItem.VerifyTypeID.Value).FirstOrDefault();
            }
         
        }
        public IEnumerable<ReceiveReturnBusinessTripCardDataView> GetReceiveReturnBusinessTripCardDataViews(BusinessTripCardDataSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Cards.GetDataReturnRetrieveBusinessCard(criteria).ToList();
            }
        }

        public IEnumerable<ReceiveReturnVisitorCardDataView> GetReceiveReturnVisitorCardDataViews(VisitorCardDataSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                var dataItems = unitOfWork.Cards.GetDataReturnRetrieveVisitorCard(criteria).ToList();
                foreach(var data in dataItems)
                {
                    LoadData(unitOfWork, data);
                }
                return new PagedList<ReceiveReturnVisitorCardDataView>(dataItems, 1, dataItems.Count());
            }
        }

        /// <summary>
        /// Update Transaction from return card. 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ObjectResult ReturnVisitorCard(ReceiveReturnVisitorCardDataView entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    var transaction = unitOfWork.AcsTransactions.Get(entity.TranID);
                    transaction.CardReturnTime = DateTime.Now;
                    transaction.UpdateBy = entity.UpdateBy;
                    transaction.UpdateDate = DateTime.Now;
                    unitOfWork.AcsTransactions.Edit(transaction);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        /// <summary>
        /// Update Transaction from receive card.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ObjectResult ReceiveVisitorCard(ReceiveReturnVisitorCardDataView entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    // Update transaction 
                    var transaction = unitOfWork.AcsTransactions.Get(entity.TranID);
                    transaction.CardReceiveTime = DateTime.Now;
                    transaction.UpdateBy = entity.UpdateBy;
                    transaction.UpdateDate = DateTime.Now;
                    unitOfWork.AcsTransactions.Edit(transaction);
                    // Update AcsVisitorDetail
                    //var detail = unitOfWork.AcsVisitorDetails.Get(entity.DetailID);
                    //detail.VerifyTypeID = entity.VerifyTypeID.HasValue ? entity.VerifyTypeID.Value : 0;
                    //detail.VerifyNo = entity.VerifyNo;
                    //unitOfWork.AcsVisitorDetails.Edit(detail);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }
       
        public ObjectResult ReturnBusinessCard(ReceiveReturnBusinessTripCardDataView entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    //unitOfWork.Cards.ReturnBusinessCard(entity);
                    var transaction = unitOfWork.AcsTransactions.Get(entity.TranID);
                    transaction.CardReturnTime = DateTime.Now;
                    transaction.UpdateBy = entity.UpdateBy;
                    transaction.UpdateDate = DateTime.Now;
                    unitOfWork.AcsTransactions.Edit(transaction);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult ReceiveBusinessCard(ReceiveReturnBusinessTripCardDataView entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    //unitOfWork.Cards.ReceiveBusinessCard(entity);
                    var transaction = unitOfWork.AcsTransactions.Get(entity.TranID);
                    transaction.CardReceiveTime = DateTime.Now;
                    transaction.UpdateBy = entity.UpdateBy;
                    transaction.UpdateDate = DateTime.Now;
                    unitOfWork.AcsTransactions.Edit(transaction);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }
        #endregion

        #region Item
        public Item GetItem(int ItemID)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Items.Get(ItemID);
            }
        }
        public IEnumerable<Item> GetAllItems()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Items.GetAll();
            }
        }
        public IEnumerable<ItemDataView> GetItemViewsByCriteria(ItemSearchCriteria criteria)
        {
            criteria.EnsureDataValid();
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Items.GetByCriteria(criteria).ToList();
            }
        }
        public ObjectResult CreateItem(Item entity)
        {

            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    if (unitOfWork.Items.IsDuplicate(entity))
                    {
                        throw new DuplicateDataException();
                    }
                    else
                    {

                        unitOfWork.Items.Add(entity);
                        unitOfWork.Complete();
                        return ObjectResult.Succeed();
                    }
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }

        }

        public ObjectResult UpdateItem(Item entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    unitOfWork.Items.Edit(entity);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }

        }

        public ObjectResult DeleteItem(Item entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    if (unitOfWork.Items.IsInUse(entity.ItemID))
                    {
                        throw new ItemInUseException();
                    }

                    unitOfWork.Items.Remove(entity);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }
        #endregion

        #region Gate
        public IEnumerable<Gate> GetGatesByFactoryCode(string factoryCode)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Gates.Find(t => t.FactoryCode == factoryCode).ToList();
            }
        }

        public IEnumerable<Gate> GetActiveGates()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Gates.Find(t => t.IsActive).ToList();
            }
        }

        public Gate GetGate(string gateID)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Gates.Find(t => String.Compare(t.GateID, gateID, true) == 0).FirstOrDefault();
            }
        }

        public IEnumerable<Gate> GetAllGates()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Gates.GetAll();
            }
        }

        public ObjectResult CreateGate(Gate entity)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                unitOfWork.Gates.Add(entity);
                unitOfWork.Complete();
                return ObjectResult.Succeed();
            }
        }

        public ObjectResult UpdateGate(Gate entity)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                unitOfWork.Gates.Edit(entity);
                unitOfWork.Complete();
                return ObjectResult.Succeed();
            }
        }

        public ObjectResult DeleteGate(Gate entity)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                unitOfWork.Gates.Remove(entity);
                unitOfWork.Complete();
                return ObjectResult.Succeed();
            }
        }
        #endregion

        #region Misc
        public Misc GetMisc(int id)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Miscs.Get(id);
            }
        }

        public IEnumerable<Misc> GetMiscsByType(string type)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                var criteria = new MiscSearchCriteria() { Type = type, Status = MiscStatus.Active  };
                return unitOfWork.Miscs.GetBySearchCriteria(criteria).ToList();
            }
        }

        public IEnumerable<Misc> GetMiscsByCriteria(MiscSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                criteria.EnsureDataValid();
                return unitOfWork.Miscs.GetBySearchCriteria(criteria).ToList();
            }
        }

        public ObjectResult CreateMisc(Misc entity)
        {
            try
            {
                entity.CreateDate = DateTime.Now;
                entity.UpdateDate = DateTime.Now;
                using (var unitOfWork = CreateUnitOfWork())
                {
                    if (unitOfWork.Miscs.IsDuplicate(entity))
                    {
                        throw new DuplicateDataException();
                    }
                    unitOfWork.Miscs.Add(entity);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult UpdateMisc(Misc entity)
        {
            try
            {
                entity.UpdateDate = DateTime.Now;
                using (var unitOfWork = CreateUnitOfWork())
                {
                    //if (unitOfWork.Miscs.IsDuplicate(entity))
                    //{
                    //    throw new DuplicateDataException();
                    //}
                    unitOfWork.Miscs.Edit(entity);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }

        public ObjectResult DeleteMisc(Misc entity)
        {
            try
            {
                using (var unitOfWork = CreateUnitOfWork())
                {
                    if (unitOfWork.Miscs.IsInUse(entity.MiscID))
                    {
                        throw new MiscInUseException();
                    }
                    unitOfWork.Miscs.Remove(entity);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();
                }
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }


        #endregion

        #region System Misc
        public SystemMisc GetSystemMisc(string type, string code)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.SystemMiscs.Find(t => String.Compare(t.SysMiscType , type,true)==0 && String.Compare( t.SysMiscCode , code,true)==0).FirstOrDefault();
            }
        }

        public IEnumerable<SystemMisc> GetSystemMiscsByMiscType(string miscType)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.SystemMiscs.GetByMiscType(miscType);
            }
        }
        public IEnumerable<SystemMisc> GetCardTypeForCreate()
        {
             using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.SystemMiscs.GetCardTypeForCreate();
            }
        }
        public IEnumerable<SystemMisc> GetCardTypeForSearch()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.SystemMiscs.GetCardTypeForSearch();
            }
        }
        public  EntryTimeSetting GetDefaultEntryTime()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.SystemMiscs.GetDefaultEntryTime().FirstOrDefault();
            }
        }
        #endregion

        #region Employee
        public IEnumerable<Employee> GetAllEmployee()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.GetAll();
            }
        }

        public IEnumerable<Employee> GetEmployeePagedListByCriteria(EmployeeSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.GetPagedListByCriteria(criteria);                
            }
        }
        
        public EmployeeInformation GetEmployeeInformation(string user)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.GetInformation(user);
            }
        }

        public Employee GetEmployee(string employeeId)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.Find(t => String.Compare(t.EmpID, employeeId, true) == 0).FirstOrDefault();
            }

        }

        public EmployeeDataView GetEmployeeDataView(string employeeId)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.Find(new EmployeeSearchCriteria { EmployeeID = employeeId, Filter = EmployeeFilterTypes.EmployeeID }).FirstOrDefault();
            }

        }
        public IEnumerable<Employee> FindEmployee(Expression<Func<Employee, bool>> predicate)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.Find(predicate).ToList();
            }
        }

        public IEnumerable<EmployeeDataView> FindEmployeeDataViews(EmployeeSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.Find(criteria).ToList();
            }
        }

        private void LoadData(IEmployee employee,LoadEmployeeOptions loadOptions)
        {
            if ((loadOptions & LoadEmployeeOptions.UserGroups)>0)
            {

            }

            if ((loadOptions & LoadEmployeeOptions.Area) > 0)
            {
                
            }
        }

        public IEnumerable<Department> GetAllDepartment()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Departments.GetAll().ToList();
            }
        }

        public Department GetDepartment(string id)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Departments.Get(id);
            }
        }

        public IEnumerable<Position> GetAllPosition()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Positions.GetAll().ToList();
            }
        }

        public Position GetPosition(string id)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Positions.Get(id);
            }
        }

        public IEnumerable<Position> GetAllSpecialPosition()
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.SystemMiscs.GetByMiscType(SystemMiscTypes.SpecialPosition)
                    .Select(t=> new Position() { PositionID = t.SysMiscCode , NameEN = t.SysMiscValue1, NameTH = t.SysMiscValue2  })
                    .ToList();
            }
        }

        public IEnumerable<EmployeeDataView> GetEmployeeDataViewsByCriteria(EmployeeSearchCriteria criteria)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                return unitOfWork.Employees.GetEmployeeByCriteria(criteria);
            }
        }

        public ObjectResult CreateEmployee(Employee entity)
        {
            using (var unitOfWork = CreateUnitOfWork())
            {
                try
                {

                    unitOfWork.Employees.Add(entity);
                    // Remain insert Area
                    unitOfWork.AreaCardMappings.Deletes(entity.CardID);
                    foreach (var areaMapping in entity.AreaMappings)
                    {
                        unitOfWork.AreaCardMappings.Add(new AreaCardMapping() { AreaID = areaMapping.AreaID, CardID = areaMapping.CardID, IsMainArea = areaMapping.IsMainArea });
                    }
                    // Create User 
                    int userId = unitOfWork.Employees.CreateUser(entity);
                    // Create Role (Officer)
                    unitOfWork.Employees.CreateEmployeeRoleMapping(userId);
                    unitOfWork.Complete();
                    return ObjectResult.Succeed();

                }
                catch (Exception ex)
                {
                    unitOfWork.AreaCardMappings.Deletes(entity.CardID);
                    unitOfWork.Employees.Remove(entity);
                    return ObjectResult.Fail(ex);
                }
            }
           
        }

        public ObjectResult UpdateEmployee(Employee entity)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    entity.UpdateDate = DateTime.Now;
                    u.Employees.Edit(entity);

                    if (entity.UserRoles != null && entity.UserRoles.Count() > 0)
                    {
                        // Update User Group
                        u.Employees.UpdateRoleUserMapping(entity.EmpID, entity.UserRoles);
                    }

                    if (entity.AreaMappings != null && entity.AreaMappings.Count() > 0)
                    {
                        // Update Area Mapping
                        u.Employees.UpdateAreaMapping(entity.EmpID, entity.AreaMappings.ToArray());
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

        public ObjectResult CreateUser(Employee employee)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    u.Employees.CreateUser(employee);
                    u.Complete();
                }
                return ObjectResult.Succeed();
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
        }
        #endregion

        #region User Role
        public IEnumerable<UserRole> GetUserRolesByEmployeeID(string employeeId)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.Employees.GetUserRolesByEmployeeID(employeeId);
            }
        }
        #endregion

        public ObjectResult CreateAreaCardMapping(AreaCardMapping mapping)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    u.AreaCardMappings.Add(mapping);
                }
                return ObjectResult.Succeed();
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
            
        }

        public ObjectResult DeleteAreaCardMappings(string cardId)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    u.AreaCardMappings.Deletes(cardId);
                }
                return ObjectResult.Succeed();
            }
            catch (Exception ex)
            {
                return ObjectResult.Fail(ex);
            }
            
        }

        public IEnumerable<AcsTask> GetAllAcsTask()
        {
            using (var u = CreateUnitOfWork())
            {
                return u.AcsTasks.GetAll();
            }
        }

        public ObjectResult UpdateAcsTask(AcsTask task)
        {
            try
            {
                using (var u = CreateUnitOfWork())
                {
                    var entities = u.AcsTasks.Find(t=>t.TaskID == task.TaskID);
                    foreach (var entity in entities)
                    {
                        entity.Success += task.IsSuccess ? 1 : 0;
                        entity.Fail += task.IsSuccess ? 0 : 1;
                        entity.IsLastResultSuccess = task.Error == null;
                        entity.LastResultMessage = task.LastResultMessage;
                        entity.LastRunDateTime = DateTime.Now;
                        entity.UpdateBy = task.UpdateBy;
                        u.AcsTasks.Edit(entity);
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

        public int[] GetAreaFromOrganizeMappings(AreaOrganizeSearchCriteria criteria)
        {
            using (var u = CreateUnitOfWork())
            {
                return u.AreaOrganizeMappings.GetBySearchCriteria(criteria).Select(t => t.AreaID).ToArray();
            }
        }
    }
}
