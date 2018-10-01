using CSI.Data.Extensions;
using CSI.IO;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Excel;
using Newtonsoft.Json;
using SECOM.ACS.Models;
using SECOM.ACS.Services;
using SECOM.ACS.Tasks.ClassMaps;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SECOM.ACS.Tasks
{
    [AcsTaskAttribute(CanEdit = true, CanRun = true)]
    public class UpdateEmployeeInfoTask : AcsInterfaceFileTaskBase<UpdateEmployeeInfoTaskOptions>
    {
        private readonly IAccessControlService service;
        private readonly IDataInterfaceService interfaceService;

        public UpdateEmployeeInfoTask(IAccessControlService service,IDataInterfaceService interfaceService) 
            : base("ACP010", "Update Employee Information")
        {
            this.service = service;
            this.interfaceService = interfaceService;
        }

        protected override object ExecuteTask(UpdateEmployeeInfoTaskOptions options)
        {
            var employeeToImports = new List<EmployeeImportData>();
            if (options.TaskOptions.EmployeeToImports == null)
            {
                if (String.IsNullOrEmpty(options.TaskOptions.ImportFile))
                {
                    throw new ArgumentNullException("ImportFile", "Import file is not specified.");
                }

                var filePath = FileHelper.GetApplicationFullPath(options.TaskOptions.ImportFile);
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"Could not load employee data file {filePath}. Employee data file not found", filePath);
                }

                // Read excel file.
                var configuration = new CsvConfiguration() { HasHeaderRecord = options.TaskOptions.UseHeaderRow };
                configuration.RegisterClassMap(new EmployeeImportDataMap());
                using (var reader = new CsvReader(new ExcelParser(filePath,configuration)))
                {
                    employeeToImports = reader.GetRecords<EmployeeImportData>().ToList();
                }
            }
            else {
                employeeToImports = options.TaskOptions.EmployeeToImports.ToList();
            }

            // Nothing to do next step.
            if (employeeToImports.Count == 0) {
                OnProgress(new TaskProgressEventArgs($"No employee data to import."));
                return employeeToImports;
            }

            // Cache data for validate.
            var positions = service.GetAllPosition();
            var specialPositions = service.GetAllSpecialPosition();
            var departments = service.GetAllDepartment();
            var employees = service.GetAllEmployee();
            var areas = service.GetAllArea();

            foreach (var employeeToImport in employeeToImports)
            {
                OnProgress(new TaskProgressEventArgs($"Preparing employee data to perform import."));
                employeeToImport.EnsureTrimmingString();
                OnProgress(new TaskProgressEventArgs($"Employee To Import data: {JsonConvert.SerializeObject(employeeToImport)}."));

                // Validate Department
                var findDepartment = departments.FirstOrDefault(t => String.Compare(t.NameEN, employeeToImport.Department , true) == 0);
                if (findDepartment == null)
                {
                    // set flag for invalid data.
                    var message = $"Invalid department name. Department: {employeeToImport.Department} is not found in deparment master data.";
                    employeeToImport.AddError(message);
                    OnProgress(new TaskProgressEventArgs(message));
                }

                // Validate position
                var findPosition = positions.FirstOrDefault(t => String.Compare(t.NameEN, employeeToImport.Position, true) == 0);
                if (findPosition==null)
                {
                    // set flag for invalid data.
                    var message = $"Invalid position name. Position: {employeeToImport.Position} is not found in position master data.";
                    employeeToImport.AddError(message);
                    OnProgress(new TaskProgressEventArgs(message));
                }

                // Validate special position
                Position findSpecialPosition = null;
                if (!String.IsNullOrEmpty(employeeToImport.SpecialPosition))
                {
                    findSpecialPosition = specialPositions.FirstOrDefault(t => String.Compare(t.NameEN, employeeToImport.SpecialPosition , true) == 0);
                    if (findSpecialPosition == null)
                    {
                        // set flag for invalid data.
                        var message = $"Invalid special position name. Special position: {employeeToImport.SpecialPosition} is not found in system misc data.";
                        employeeToImport.AddError(message);
                        OnProgress(new TaskProgressEventArgs(message));
                    }
                }

                // Validate Area
                var validateResults = employeeToImport.ValidateArea(areas.ToArray());
                if (!validateResults.IsSucceed) {
                    var message = String.Join(", ", validateResults.GetErrorMessages());
                    employeeToImport.AddError(message);
                    OnProgress(new TaskProgressEventArgs(message));
                }

                if (!employeeToImport.IsValid) { continue; }
                // Setting Master data.
                employeeToImport.DepartmentMaster = findDepartment;
                employeeToImport.PositionMaster = findPosition;
                employeeToImport.SpecialPositionMaster = findSpecialPosition;

                var employee = employeeToImport.ToEmployee(options.TaskOptions.User, areas.ToArray());
                // Load AreaID from AreaOrganizeMapping
                var areaMappingFromOrganize = service.GetAreaFromOrganizeMappings(employeeToImport.ToAreaOrganizeCriteria());
                employee.MergeArea(areaMappingFromOrganize);

                var card = employeeToImport.ToCard(options.TaskOptions.User);
                // Checking card is exist in card master.
                bool cardInserted = false;
                var findCard = service.GetCard(card.CardID);
                if (findCard == null)
                {
                    // Card not exists in master.
                    var cardCreateResult = service.CreateCard(card);
                    if (cardCreateResult.IsSucceed)
                    {
                        cardInserted = true;
                    }
                }

                var findEmployee = employees.FirstOrDefault(t => String.Compare(t.EmpID , employee.EmpID,true)==0);
                if (findEmployee == null)
                {
                    // Create new employee and user
                    var result = service.CreateEmployee(employee);
                    if (!result.IsSucceed)
                    {
                        if (cardInserted) {
                            service.DeleteCard(new Card() { CardID = employee.CardID });
                        }
                        employeeToImport.AddError(result.Error);
                        OnError(new ErrorEventArgs(new Exception(result.GetErrorMessage())));
                        continue;
                    }
                }
                else
                {
                    // Update Employee
                    var result = service.UpdateEmployee(employee);
                    if (!result.IsSucceed)
                    {
                        if (cardInserted)
                        {
                            service.DeleteCard(new Card() { CardID = employee.CardID });
                        }
                        employeeToImport.AddError(result.Error);
                        OnError(new ErrorEventArgs(new Exception(result.GetErrorMessage())));
                        continue;
                    }

                    OnProgress(new TaskProgressEventArgs($"Update employee is success. (Employee ID: {employee.EmpID})"));
                    if (findEmployee.CardID != employee.CardID)
                    {
                        // Delete old card id from area card mapping
                        OnProgress(new TaskProgressEventArgs($"Deleteing old card id {findEmployee.CardID}."));
                        service.DeleteAreaCardMappings(findEmployee.CardID);
                    }
                }
            }

            var totalRecords = employeeToImports.Where(t => t.IsValid).Count();
            if (totalRecords > 0)
            {
                OnProgress(new TaskProgressEventArgs($"Import employee data is finished. Total Success {totalRecords} of {employeeToImports.Count} records"));

                OnProgress(new TaskProgressEventArgs($"Creating interface file for import to Access Control System"));
                // Get data to create interface file.
                var employeesToImportAcs = employeeToImports.Where(t => t.IsValid).Select(t => t.EmpID).ToArray();
                OnProgress(new TaskProgressEventArgs($"Loading employee data for create acs interface file. Employees {String.Join(",", employeesToImportAcs)}"));
                var dataItems = interfaceService.GetEmployeesForImportAcs(employeesToImportAcs).ToList();

                // Perform create interface file -> copy to share folder, finally archive interface file.
                var interfaceResult = PerformExportInterfaceFile(options, dataItems);

                if (options.TaskOptions.EnabledBackup)
                {
                    var filePath = FileHelper.GetApplicationFullPath(options.TaskOptions.ImportFile);
                    if (File.Exists(filePath))
                    {
                        // Move employee import file to back up folder.
                        var backupFileName = String.Format(options.TaskOptions.BackupFileName, DateTime.Now);
                        var backupFile = Path.Combine(FileHelper.GetApplicationFullPath(options.TaskOptions.BackupFolder), backupFileName);
                        DirectoryHelper.EnsureDirectoryCreated(backupFile);
                        File.Move(filePath, backupFile);
                        OnProgress(new TaskProgressEventArgs($"Copy employee import file to back up folder. Output file {backupFile}"));
                    }
                }
            }
            else {
                OnProgress(new TaskProgressEventArgs($"No employee data import to system."));
            }
            return employeeToImports;
        }
    }

    

    

   
}
