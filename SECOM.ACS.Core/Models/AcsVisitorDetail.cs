//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SECOM.ACS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class AcsVisitorDetail
    {
        public System.Guid DetailID { get; set; }
        public string ReqNo { get; set; }
        public short Seq { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public string DeptName { get; set; }
        public string ItemInOut { get; set; }
        public string Photographing { get; set; }
        public string Description { get; set; }
        public Nullable<int> VerifyTypeID { get; set; }
        public string VerifyNo { get; set; }
    
        public virtual AcsVisitor AcsVisitor { get; set; }
    }
}