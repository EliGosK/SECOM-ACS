using CSI.ComponentModel;
using CSI.IO;
using log4net;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    [AcsTaskAttribute(CanEdit = true, CanRun = true)]
    public class ExportInterfaceFileToAccessControlTask : AcsInterfaceFileTaskBase<ExportInterfaceFileToAccessControlTaskOptions>
    {
        private readonly IDataInterfaceService interfaceService;
        private readonly IAccessControlService service;

        public ExportInterfaceFileToAccessControlTask(IDataInterfaceService interfaceService,IAccessControlService service) 
            : base("ACP030", "Export interface file to Access Control System")
        {
            this.interfaceService = interfaceService;
            this.service = service;
        }

        protected override object ExecuteTask(ExportInterfaceFileToAccessControlTaskOptions options)
        {
            switch (options.TaskOptions.ExportModes)
            {
                case ExportToAccessControlModes.Employee:
                    return ExportInterfaceFileForEmployee(options);
                case ExportToAccessControlModes.Add:
                    return ExportInterfaceFileForAdd(options);
                case ExportToAccessControlModes.Cancel:
                    return ExportInterfaceFileForCancel(options);
                case ExportToAccessControlModes.Schedule:
                    return ExportInterfaceFileForSchedule(options);
            }
            return null;
        }

        private object ExportInterfaceFileForEmployee(ExportInterfaceFileToAccessControlTaskOptions options)
        {
            if (options.TaskOptions.Employees == null || options.TaskOptions.Employees.Length == 0) { return null; }
            OnProgress(new TaskProgressEventArgs($"Loading access control data to create acs interface file (EMPLOYEE). Employee { String.Join(", ", options.TaskOptions.Employees)})"));
            var dataItems = interfaceService.GetEmployeesForImportAcs(options.TaskOptions.Employees).ToList();
            // Perform create interface file -> copy to share folder, finally archive interface.
            var interfaceResult = PerformExportInterfaceFile(options, dataItems);
            if (!interfaceResult.IsSucceed)
            {
                throw interfaceResult.Error;
            }
            return interfaceResult.DataState;

        }

        private object ExportInterfaceFileForAdd(ExportInterfaceFileToAccessControlTaskOptions options)
        {
            if (options.TaskOptions.Transactions == null || options.TaskOptions.Transactions.Length == 0)
            {
                OnProgress(new TaskProgressEventArgs($"No transactions to create acs interface file"));
                return null;
            }

            var transactionsToAdd = options.TaskOptions.Transactions.Select(t => t.ToString()).ToArray();
            OnProgress(new TaskProgressEventArgs($"Loading access control data to create acs interface file (ADD). Transaction {String.Join(",", transactionsToAdd)})"));
            var dataItems = interfaceService.GetEmployeesForImportAcsToAdd(options.TaskOptions.EffectiveDate, transactionsToAdd).ToList();
            if (dataItems.Count > 0)
            {
                // Perform create interface file -> copy to share folder, finally archive interface.
                var interfaceResult = PerformExportInterfaceFile(options, dataItems);
                if (interfaceResult.IsSucceed)
                {
                    // Update SendAcsData = Now
                    foreach (var tran in options.TaskOptions.Transactions)
                    {
                        TransactionAcs tranDataItem = service.GetAcsTransaction(tran);
                        if (tranDataItem == null) { continue; }
                        tranDataItem.Status = (byte)TransactionStatus.SendCardToACS;
                        tranDataItem.SendAcsDate = DateTime.Now;
                        tranDataItem.CancelAcsDate = null;
                        tranDataItem.UpdateBy = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                        tranDataItem.UpdateDate = DateTime.Now;
                        // Update Transaction
                        service.UpdateAcsTransactions(tranDataItem);
                    }
                    return interfaceResult.DataState;
                }
                else
                {
                    throw interfaceResult.Error;
                }
            }
            return null;
        }

        private object ExportInterfaceFileForCancel(ExportInterfaceFileToAccessControlTaskOptions options)
        {
            if (options.TaskOptions.Transactions == null || options.TaskOptions.Transactions.Length == 0)
            {
                OnProgress(new TaskProgressEventArgs($"No transactions to create acs interface file"));
                return null;
            }

            var transactionsToCancel = options.TaskOptions.Transactions.Select(t => t.ToString()).ToArray();
            OnProgress(new TaskProgressEventArgs($"Loading access control data to create acs interface file (CANCEL). Transaction {String.Join(",", transactionsToCancel)})"));
            var dataItems = interfaceService.GetEmployeesForImportAcsToCancel(transactionsToCancel).ToList();
            // Perform create interface file -> copy to share folder, finally archive interface.
            var interfaceResult = PerformExportInterfaceFile(options, dataItems);
            if (interfaceResult.IsSucceed)
            {
                foreach (var tran in options.TaskOptions.Transactions)
                {
                    TransactionAcs tranDataItem = service.GetAcsTransaction(tran);
                    if (tranDataItem == null) { continue; }
                    if (tranDataItem.Status == (byte)TransactionStatus.SendCardToACS)
                    {
                        // Change Status 2 => 4
                        tranDataItem.Status = (byte)TransactionStatus.SendCardToCancel;
                        tranDataItem.CancelAcsDate = DateTime.Now;
                        tranDataItem.UpdateBy = System.Threading.Thread.CurrentPrincipal.Identity.Name;
                        tranDataItem.UpdateDate = DateTime.Now;
                        // Update Transaction
                        service.UpdateAcsTransactions(tranDataItem);
                    }
                }
                return interfaceResult.DataState;
            }
            else
            {
                throw interfaceResult.Error;
            }
        }

        private object ExportInterfaceFileForSchedule(ExportInterfaceFileToAccessControlTaskOptions options)
        {
            OnProgress(new TaskProgressEventArgs($"Loading access control data to create acs interface file (SCHEDULE). Effective Date {options.TaskOptions.EffectiveDate})"));
            var dataItems = interfaceService.GetEmployeesForImportAcsByEffectiveDate(options.TaskOptions.EffectiveDate).ToList();
            // Perform create interface file -> copy to share folder, finally archive interface.
            var interfaceResult = PerformExportInterfaceFile(options, dataItems);
            if (!interfaceResult.IsSucceed)
            {
                throw interfaceResult.Error;
            }
            return interfaceResult.DataState;
        }

        

    }

}
