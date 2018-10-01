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
    public interface IDocumentService
    {
        UpdateDocumentResult UpdateDocumentStatus(string user,int documentType);
    }

    public class UpdateDocumentResult : ObjectResult<UpdateDocumentExpirationData>
    {
        public UpdateDocumentResult(UpdateDocumentExpirationData data) : base(data)
        {

        }

        public UpdateDocumentResult(UpdateDocumentExpirationData data,Exception ex) : base(data,ex)
        {

        }
    }

    public class UpdateDocumentExpirationData
    {
        public UpdateDocumentExpirationData()
        {
            
        }

        public UpdateDocumentExpirationData(IAcsRequest[] expireRequestNoList, IAcsRequest[] doneRequestNoList)
        {
            this.ExpireRequestNoList = expireRequestNoList;
            this.DoneRequestNoList = doneRequestNoList;
        }

        public IAcsRequest[] ExpireRequestNoList { get; private set; }
        public IAcsRequest[] DoneRequestNoList { get; private set; }
    }
}
