﻿using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Data
{
    public interface IReqAreaMappingRepository : IRepository<ReqAreaMapping>
    {
        void RemovesByRequestNo(string requestNo);
    }
}
