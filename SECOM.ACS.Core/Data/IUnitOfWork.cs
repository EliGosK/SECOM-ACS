using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IUnitOfWork : IDisposable
    {
        IAreaRepository Areas { get; }
        ICardRepository Cards { get; }
        IItemRepository Items { get; }
        IMiscRepository Miscs { get; }
        ISystemMiscRepository SystemMiscs { get; }
        IGateRepository Gates { get; }
        IPermissionRecordRepository PermissionRecords { get; }
        IPermissionDashboardRepository PermissionDashboards { get; }
        IAcsEmployeeRepository AcsEmployees { get; }
        IAcsEmployeeDetailRepository AcsEmployeeDetails { get; }
        IAcsItemInRepository AcsItemIns { get; }
        IAcsItemInDetailRepository AcsItemInDetails { get; }
        IAcsItemOutRepository AcsItemOuts { get; }
        IAcsItemOutDetailRepository AcsItemOutDetails { get; }
        IAcsPhotoRepository AcsPhotos { get; }
        IAcsVIPRepository AcsVIPs { get; }
        IAcsVisitorRepository AcsVisitors { get; }
        IAcsVisitorDetailRepository AcsVisitorDetails { get; }
        IAcsRequestInquriyRepository RequestInquiry { get; }
        IReqApproverListRepository ReqApprovers { get; }
        IReqAreaMappingRepository ReqAreaMappings { get; }
        IAcsTransactionRepository AcsTransactions { get; }
        IAreaCardMappingRepository AreaCardMappings { get; }
        IAreaOrganizeMappingRepository AreaOrganizeMappings { get; }
        IAreaApproverRepository AreaApprovers { get; }
        IEmployeeRepository Employees { get; }
        IPositionRepository Positions { get; }
        IDepartmentRepository Departments { get; }
        IAcsTaskRepository AcsTasks { get; }
        int Complete();
    }
}
