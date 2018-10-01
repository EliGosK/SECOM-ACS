using SECOM.ACS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public interface IImportToAcsTask
    {
        void Execute(ImportToAcsOptions options);
    }

    public class ImportToAcsTask : AcsTaskBase<ImportToAcsOptions>, IImportToAcsTask
    {
        private readonly IAccessControlService service;
        public ImportToAcsTask(IAccessControlService service) : base("ACP010", "Update Employee Information")
        {
            this.service = service;
        }

        protected override void ExecuteTask(ImportToAcsOptions options)
        {
            throw new NotImplementedException();
        }

    }

    
}
