using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface ICardRepository : IRepository<Card, string, CardSearchCriteria>
    {
        bool IsInUse(string CardID);
        bool IsDuplicate(Card entity);
        void LoadArea(Card card);

        IEnumerable<Card> GetCardForCheckData(string id);

        void AddOrUpdate(Card card);
        IEnumerable<VisitorCardRegistrationView> GetVisitorCardRegistrationViewsByCriteria(VisitorCardRegistrationSearchCriteria criteria);
        void UpdateVisitorCardRegistrationView(VisitorCardRegistrationView entity);

        IEnumerable<BusinessTripCardRegistrationView> GetBusinessTripCardRegistrationViewsByCriteria(BusinessTripCardRegistrationSearchCriteria criteria);
        void UpdateBusinessTripCardRegistrationView(BusinessTripCardRegistrationView entity);

        IEnumerable<CardNoView> GetCardNoList(RegisterCardSearchCriteria criteria);
        IEnumerable<CheckOverlapCardNoList> CheckOverlapCardNo(DateTime EntryDate, String CardNo);
        IEnumerable<CheckOverlapPeriodCardNoList> CheckOverlapPeriodCardNo(DateTime EntryDateFrom, DateTime EntryDateTo, String CardNo);

        IEnumerable<ReceiveReturnVisitorCardDataView> GetDataReturnRetrieveVisitorCard(VisitorCardDataSearchCriteria criteria);
        IEnumerable<ReceiveReturnBusinessTripCardDataView> GetDataReturnRetrieveBusinessCard(BusinessTripCardDataSearchCriteria criteria);

        void ReceiveBusinessCard(ReceiveReturnBusinessTripCardDataView model);
        void ReturnBusinessCard(ReceiveReturnBusinessTripCardDataView model);

        void ReceiveVisitorCard(ReceiveReturnVisitorCardDataView model);
        void ReturnVisitorCard(ReceiveReturnVisitorCardDataView model);
    }
}
