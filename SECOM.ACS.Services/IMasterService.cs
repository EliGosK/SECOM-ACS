using CSI.ComponentModel;
using CSI.Core;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Services
{
    public interface IMasterService
    {
        Item GetItem(int ItemID);
        IEnumerable<Item> GetAllItems();
        IEnumerable<ItemDataView> GetItemViewsByCriteria(ItemSearchCriteria criteria);
        ObjectResult CreateItem(Item entity);
        ObjectResult UpdateItem(Item entity);
        ObjectResult DeleteItem(Item entity);

        Area GetArea(int id);
        IEnumerable<Area> GetAllArea();
        IEnumerable<Area> GetAreaByCriteria(AreaSearchCriteria criteria, AreaLoadOptions loadOptions);
        IEnumerable<AreaDataView> GetAreaDataViews(string user,bool areaMapping);
        IEnumerable<AreaMapping> GetAreaMapping(string employeeId);
        IEnumerable<Position> GetPositionsForAreaApprover();
        IEnumerable<Department> GetDepartmentForAreaApprover();
        ObjectResult CreateArea(Area area);
        ObjectResult UpdateArea(Area area);
        ObjectResult DeleteArea(Area area);

        Gate GetGate(string gateID);
        IEnumerable<Gate> GetActiveGates();
        IEnumerable<Gate> GetAllGates();
        IEnumerable<Gate> GetGatesByFactoryCode(string factoryCode);
        ObjectResult CreateGate(Gate entity);
        ObjectResult UpdateGate(Gate entity);
        ObjectResult DeleteGate(Gate entity);

        IEnumerable<Card> GetCardsByCriteria(CardSearchCriteria criteria);
        Card GetCard(string id);
        ObjectResult CreateCard(Card card);
        ObjectResult UpdateCard(Card entity);
        ObjectResult DeleteCard(Card entity);
        ObjectResults<ImportCardResult> ImportCard(IEnumerable<Card> cards);

        IEnumerable<VisitorCardRegistrationView> GetVisitorCardRegistrationsByCriteria(VisitorCardRegistrationSearchCriteria criteria);
        ObjectResult UpdateVisitorCardRegistrationView(VisitorCardRegistrationView entity);

        IEnumerable<BusinessTripCardRegistrationView> GetBusinessTripCardRegistrationsByCriteria(BusinessTripCardRegistrationSearchCriteria criteria);
        ObjectResult UpdateBusinessTripCardRegistrationView(BusinessTripCardRegistrationView entity);

        IEnumerable<CardNoView> GetCardNoList(RegisterCardSearchCriteria criteria);
        ObjectResult CheckOverlapCardNo(DateTime entryDate, String cardNo);
        ObjectResult CheckOverlapPeriodCardNo(DateTime entryDateFrom, DateTime entryDateTo, String cardNo);

        int[] GetAreaFromOrganizeMappings(AreaOrganizeSearchCriteria criteria);

        Misc GetMisc(int id);
        IEnumerable<Misc> GetMiscsByType(string type);
        IEnumerable<Misc> GetMiscsByCriteria(MiscSearchCriteria criteria);
        ObjectResult CreateMisc(Misc entity);
        ObjectResult UpdateMisc(Misc entity);
        ObjectResult DeleteMisc(Misc entity);

        SystemMisc GetSystemMisc(string type,string code);
        IEnumerable<SystemMisc> GetSystemMiscsByMiscType(string name);
        IEnumerable<SystemMisc> GetCardTypeForCreate();
        IEnumerable<SystemMisc> GetCardTypeForSearch();
        EntryTimeSetting GetDefaultEntryTime();

        // Employee
        IEnumerable<Employee> GetAllEmployee();
        IEnumerable<Employee> GetEmployeePagedListByCriteria(EmployeeSearchCriteria criteria);
        EmployeeInformation GetEmployeeInformation(string userNameOrEmployeeId);
        Employee GetEmployee(string employeeId);
        IEnumerable<Employee> FindEmployee(Expression<Func<Employee, bool>> predicate);
        EmployeeDataView GetEmployeeDataView(string emplyeeId);
        IEnumerable<EmployeeDataView> FindEmployeeDataViews(EmployeeSearchCriteria criteria);
        ObjectResult CreateEmployee(Employee entity);
        ObjectResult UpdateEmployee(Employee entity);
      
        IEnumerable<EmployeeDataView> GetEmployeeDataViewsByCriteria(EmployeeSearchCriteria criteria);

        // Role
        IEnumerable<UserRole> GetUserRolesByEmployeeID(string employeeId);

        // Department
        IEnumerable<Department> GetAllDepartment();
        Department GetDepartment(string id);

        // Position
        IEnumerable<Position> GetAllPosition();
        Position GetPosition(string id);

        // Special Position
        IEnumerable<Position> GetAllSpecialPosition();

        // Area Card Mapping
        ObjectResult CreateAreaCardMapping(AreaCardMapping mapping);
        ObjectResult DeleteAreaCardMappings(string cardId);

        IEnumerable<AcsTask> GetAllAcsTask();
        ObjectResult UpdateAcsTask(AcsTask batch);


        ObjectResult CreateUser(Employee employee);
    }
}
