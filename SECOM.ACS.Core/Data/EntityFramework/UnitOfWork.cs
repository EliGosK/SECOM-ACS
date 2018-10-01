using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ACSContext _context;

        public UnitOfWork(ACSContext context)
        {
            _context = context;
            Areas = new AreaRepository(_context);
            Cards = new CardRepository(_context);
            Items = new ItemRepository(_context);
            Miscs = new MiscRepository(_context);
            SystemMiscs = new SystemMiscRepository(_context);
            Gates = new GateRepository(_context);

            PermissionRecords = new PermissionRecordRepository(_context);
            PermissionDashboards = new PermissionDashboardRepository(_context);
            AcsEmployees = new AcsEmployeeRepository(_context);
            AcsItemIns = new AcsItemInRepository(_context);
            AcsItemInDetails = new AcsItemInDetailRepository(_context);
            AcsItemOuts = new AcsItemOutRepository(_context);
            AcsItemOutDetails = new AcsItemOutDetailRepository(_context);
            AcsPhotos = new AcsPhotoRepository(_context);
            AcsVisitors = new AcsVisitorRepository(_context);
            AcsVisitorDetails = new AcsVisitorDetailRepository(_context);
            AcsVIPs = new AcsVIPRepository(_context);
            RequestInquiry = new AcsRequestInquriyRepository(_context);
            ReqApprovers = new ReqApproverListRepository(_context);
            ReqAreaMappings = new ReqAreaMappingRepository(_context);
            AcsEmployeeDetails = new AcsEmployeeDetailRepository(_context);
            AcsTransactions = new AcsTransactionRepository(_context);
            AreaCardMappings = new AreaCardMappingRepository(_context);
            AreaOrganizeMappings = new AreaOrganizeMappingRepository(_context);
            AreaApprovers = new AreaApproverRepository(_context);
            Employees = new EmployeeRepository(_context);
            Positions = new PositionRepository(_context);
            Departments = new DepartmentRepository(_context);
            AcsTasks = new AcsTaskRepository(_context);
        }

        public IAreaRepository Areas { get; private set; }
        public ICardRepository Cards { get; private set; }
        public IItemRepository Items { get; private set; }
        public IMiscRepository Miscs { get; private set; }
        public ISystemMiscRepository SystemMiscs { get; private set; }
        public IGateRepository Gates { get; private set; }
        public IPermissionRecordRepository PermissionRecords { get; private set; }
        public IPermissionDashboardRepository PermissionDashboards { get; private set; }
        public IAcsEmployeeRepository AcsEmployees { get; private set; }
        public IAcsEmployeeDetailRepository AcsEmployeeDetails { get; }
        public IAcsItemInRepository AcsItemIns { get; private set; }
        public IAcsItemInDetailRepository AcsItemInDetails { get; }
        public IAcsItemOutRepository AcsItemOuts { get; }
        public IAcsItemOutDetailRepository AcsItemOutDetails { get; }
        public IAcsPhotoRepository AcsPhotos { get; private set; }
        public IAcsVisitorRepository AcsVisitors { get; private set; }
        public IAcsVisitorDetailRepository AcsVisitorDetails { get; private set; }
        public IAcsVIPRepository AcsVIPs { get; private set; }
        public IReqApproverListRepository ReqApprovers { get; private set; }
        public IReqAreaMappingRepository ReqAreaMappings { get; private set; }
        public IAcsRequestInquriyRepository RequestInquiry { get; private set; }
        public IAcsTransactionRepository AcsTransactions { get; private set; }
        public IAreaCardMappingRepository AreaCardMappings { get; private set; }
        public IAreaOrganizeMappingRepository AreaOrganizeMappings { get; private set; }
        public IAreaApproverRepository AreaApprovers { get; }
        public IEmployeeRepository Employees { get; }
        public IPositionRepository Positions { get; }
        public IDepartmentRepository Departments { get; }
        public IAcsTaskRepository AcsTasks { get; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
