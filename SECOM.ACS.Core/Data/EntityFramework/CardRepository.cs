using System;
using System.Collections.Generic;
using CSI.Data.EntityFramework;
using SECOM.ACS.Models;
using System.Linq;
using System.Data.Entity;
using CSI.ComponentModel;

namespace SECOM.ACS.Data.EntityFramework
{
    public class CardRepository : EntityRepository<ACSContext, Card, string>, ICardRepository
    {
        public CardRepository(ACSContext context) : base(context)
        {

        }

        public IEnumerable<Card> GetBySearchCriteria(CardSearchCriteria criteria)
        {
            var datas = Context.GetCardsByCriteria(criteria.CardNo, criteria.CardTypeValue, criteria.StatusValue).ToList();
            foreach (var data in datas)
            {
                LoadArea(data);
            }
            return datas;
        }

        public IEnumerable<Card> GetCardForCheckData(string id)
        {
            return Context.Cards.Where(t => (String.Compare(t.CardID, id) == 0));
        }

        public override void Add(Card entity)
        {
            Context.InsertCard(entity.CardID, entity.CardType, entity.CardNo, entity.Note, entity.IsActive, entity.CreateBy);
            foreach (var card in entity.AreaCardMappings)
            {
                Context.InsertAreaCardMapping(entity.CardID, card.AreaID, card.IsMainArea);
            }
        }

        public void LoadArea(Card card)
        {
            var areaCardMappings = Context.AreaCardMappings.Where(t => t.CardID == card.CardID).ToList();
            foreach (var areaCardMapping in areaCardMappings)
            {
                card.AreaCardMappings.Add(areaCardMapping);
            }
        }

        public override void Edit(Card entity)
        {
            // Update Area
            Context.UpdateCard(entity.CardID, entity.CardType, entity.CardNo, entity.Note, entity.IsActive, entity.UpdateBy);

            // Determine card is modified
            if (entity.AreaCardMappings.Any(t => t.AreaID == 0))
            {
                // Clear Area Card Mapping
                Context.DeleteAreaCardMapping(entity.CardID);
                foreach (var area in entity.AreaCardMappings)
                {
                    // Insert Area Gate Mapping
                    Context.InsertAreaCardMapping(entity.CardID, area.AreaID, area.IsMainArea);
                }
            }
        }
        public override void Remove(Card entity)
        {

            Context.DeleteAreaCardMapping(entity.CardID);
            Context.DeleteCard(entity.CardID);
        }

        public bool IsInUse(string CardID)
        {
            var result = Context.CheckCardInUse(CardID).FirstOrDefault();

            if (result.HasValue)
            {
                return result.Value;
            }
            return false;
        }

        public bool IsDuplicate(Card entity)
        {
            var result = Context.CheckDuplicateCard(entity.CardNo, entity.CardType).FirstOrDefault();
            if (result.HasValue)
            {
                return result.Value == 1;
            }
            return false;
        }

        public void AddOrUpdate(Card entity)
        {
            Context.InsertOrUpdateCard(entity.CardID, entity.CardType, entity.CardNo, entity.Note, entity.CreateBy);
        }


        public IEnumerable<VisitorCardRegistrationView> GetVisitorCardRegistrationViewsByCriteria(VisitorCardRegistrationSearchCriteria criteria)
        {
            var result = Context.GetVisitorCardRegistrationViews(criteria.EntryDate, criteria.VisitorName, criteria.Company).ToList();
            return result;
        }

        public void UpdateVisitorCardRegistrationView(VisitorCardRegistrationView entity)
        {
            Context.UpdateVisitorCardRegistration(entity.TranID, entity.DetailID, entity.CardID, entity.UserName, entity.VerifyTypeID, entity.VerifyNo);
        }

        public IEnumerable<BusinessTripCardRegistrationView> GetBusinessTripCardRegistrationViewsByCriteria(BusinessTripCardRegistrationSearchCriteria criteria)
        {
            var result = Context.GetBusinessTripCardRegistrationViews(criteria.EntryDate, criteria.BusinessTripEmployeeName, criteria.RequestName).ToList();
            return result;
        }

        public void UpdateBusinessTripCardRegistrationView(BusinessTripCardRegistrationView entity)
        {
            Context.UpdateBusinessTripCardRegistration(entity.TranID, entity.CardID, entity.UserName);
        }

        public IEnumerable<CardNoView> GetCardNoList(RegisterCardSearchCriteria criteria)
        {
            var result = Context.GetCardNo(criteria.CardID, criteria.CardType).ToList();
            return result;
        }

        public IEnumerable<CheckOverlapCardNoList> CheckOverlapCardNo(DateTime entryDate, String cardNo)
        {
            var result = Context.CheckOverlapCardNo(entryDate, cardNo).ToList();
            return result;
        }

        public IEnumerable<CheckOverlapPeriodCardNoList> CheckOverlapPeriodCardNo(DateTime entryDateFrom, DateTime entryDateTo, String cardNo)
        {
            var result = Context.CheckOverlapPeriodCardNo(entryDateFrom, entryDateTo, cardNo).ToList();
            return result;
        }
        
        public IEnumerable<ReceiveReturnVisitorCardDataView> GetDataReturnRetrieveVisitorCard(VisitorCardDataSearchCriteria criteria)
        {
            var factory = criteria.Factory != null && criteria.Factory.Count() > 0 ? String.Join(",", criteria.Factory) : null;
            return Context.GetReceiveRetrunVisitorCardDataView(criteria.EntryDate,criteria.VisitorName,criteria.Company, factory).ToList();
        }

        public IEnumerable<ReceiveReturnBusinessTripCardDataView> GetDataReturnRetrieveBusinessCard(BusinessTripCardDataSearchCriteria criteria)
        {
            var factory = criteria.Factory != null && criteria.Factory.Count() > 0 ? String.Join(",", criteria.Factory) : null;
            return Context.GetReceiveReturnBusinessCardDataView(criteria.EntryDate, criteria.BusinessEmployeeName, criteria.RequesterName, factory).ToList();
        }


        public void ReceiveBusinessCard(ReceiveReturnBusinessTripCardDataView model)
        {
            Context.ReceiveBusinessTripCard(model.TranID, model.UpdateBy);
        }

        public void ReturnBusinessCard(ReceiveReturnBusinessTripCardDataView model)
        {
            Context.ReturnBusinessTripCard(model.TranID, model.UpdateBy);
        }

        public void ReceiveVisitorCard(ReceiveReturnVisitorCardDataView model)
        {
            Context.ReceiveVisitorCard(model.TranID, model.UpdateBy);
        }

        public void ReturnVisitorCard(ReceiveReturnVisitorCardDataView model)
        {
            Context.ReturnVisitorCard(model.TranID, model.UpdateBy);
        }
    }
}
