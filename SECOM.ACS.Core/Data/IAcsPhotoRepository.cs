using CSI.Data;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;

namespace SECOM.ACS.Data
{
    public interface IAcsPhotoRepository : IRepository<AcsPhoto,string>
    {
        void Update(AcsPhoto entity);
    }
}
