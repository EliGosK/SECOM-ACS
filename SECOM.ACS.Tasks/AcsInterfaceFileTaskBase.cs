using CSI.ComponentModel;
using CSI.IO;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace SECOM.ACS.Tasks
{
    public abstract class AcsInterfaceFileTaskBase<TOption> : AcsTaskBase<TOption>
        where TOption : IExportInterfaceFileTaskOptions
    {
        public AcsInterfaceFileTaskBase(string taskId, string taskName) : base(taskId, taskName)
        {

        }

        protected ObjectResult PerformExportInterfaceFile(TOption options, IList<EmployeeForImportAcs> employeesToImportAcs)
        {
            if (employeesToImportAcs.Count == 0)
            {
                OnProgress(new TaskProgressEventArgs($"Skip create acs interface file. There are no employee data to create interface file."));
                return ObjectResult.Succeed();
            }

            var outputFileName = String.Format(options.ExportInterfaceFileOptions.TargetFileName, DateTime.Now);
            var outputFile = Path.Combine(FileHelper.GetApplicationFullPath(options.ExportInterfaceFileOptions.TargetFolder), outputFileName);
            OnProgress(new TaskProgressEventArgs($"Creating acs interface file. Output file {outputFile}"));
            var fileBuilder = new AcsInterfacetFileBuilder();
            var ext = Path.GetExtension(outputFile);

            try
            {
                DirectoryHelper.EnsureDirectoryCreated(outputFile);
                if (String.Compare(ext, ".csv", true) == 0)
                {
                    // Create Csv file.
                    var csvOptions = new AcsCsvConfiguration() {
                        DateFormat = options.ExportInterfaceFileOptions.DateFormat,
                        DummyAccessGroup = options.ExportInterfaceFileOptions.DummyAccessGroup,
                        HasHeaderRecord = options.ExportInterfaceFileOptions.HasHeaderRecord
                    };
                    fileBuilder.CreateCsvReport(employeesToImportAcs, outputFile, csvOptions);
                }
                else
                {
                    // Create excel file.
                    var templateFile = FileHelper.GetApplicationFullPath(options.ExportInterfaceFileOptions.TemplateFile);
                    var reportData = new AcsImportReportData()
                    {
                        Employees = employeesToImportAcs,
                        DummyAccessGroup = options.ExportInterfaceFileOptions.DummyAccessGroup                        
                    };
                    fileBuilder.CreateExcelReport(reportData, outputFile, templateFile);
                }
                OnProgress(new TaskProgressEventArgs($"Created acs interface file is success. Output file {outputFile}"));
                return ObjectResult.Succeed(outputFile);
            }
            catch (Exception ex)
            {
                OnError(new ErrorEventArgs(ex));
                return ObjectResult.Fail(ex);
            }
            
        }
    }
}
