﻿using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IAcsItemOutDetailRepository : IRepository<AcsItemOutDetail,string>
    {
        void InsertAcsItemOutDetail(AcsItemOutDetail entity);
        void UpdateAcsItemOutDetail(AcsItemOutDetail entity);
        IEnumerable<AcsItemOutDetailDataView> GetDataViews(string requestNo);
        void DeleteAcsItemOutDetail(string RequestNo);
    }
}
