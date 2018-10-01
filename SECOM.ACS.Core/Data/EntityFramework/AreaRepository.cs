using CSI.Data.EntityFramework;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;

namespace SECOM.ACS.Data.EntityFramework
{
    public class AreaRepository : EntityRepository<ACSContext,Area, int>, IAreaRepository
    {
        public AreaRepository(ACSContext context) : base(context)
        {

        }

        public IEnumerable<Area> Find(AreaSearchCriteria criteria)
        {
            return Context.FindArea(criteria.AreaID, criteria.AreaName).ToList();
        }

        public IEnumerable<Area> GetBySearchCriteria(AreaSearchCriteria criteria)
        {
            criteria.EnsureDataValid();
            return Context.GetAreaByCriteria(criteria.FactoryCodeString, criteria.StatusValue).ToList();
        }

        public AreaDataView GetDataView(int areaID)
        {
            return Context.GetAreaDataView(areaID).FirstOrDefault();
        }

        public IEnumerable<AreaDataView> GetDataViews(string user,bool areaMapping)
        {
            return Context.GetAreaDataViews(user, areaMapping).ToList();
        }

        public void LoadAreaApprover(Area area)
        {
            if (Context.Areas.Any(t => t.AreaID == area.AreaID))
            {
                var Approver = Context.Areas.Include(t => t.AreaApprovers).Single(t => t.AreaID == area.AreaID).AreaApprovers;
                foreach (var app in Approver)
                {
                    area.AreaApprovers.Add(app);
                }
            }
        }

        public void LoadGate(Area area)
        {
            if (Context.Areas.Any(t => t.AreaID == area.AreaID))
            {
                var gates = Context.Areas.Include(t => t.Gates).Single(t => t.AreaID == area.AreaID).Gates;
                foreach (var gate in gates)
                {
                    area.Gates.Add(gate);
                }
            }
        }

        public override void Add(Area entity)
        {
            var result = Context.InsertArea(entity.FactoryCode, entity.AreaName, entity.AreaDisplayEN, entity.AreaDisplayTH, entity.ConfdtLevel, entity.IsActive, entity.CreateBy).FirstOrDefault();
            if (result != null && result.HasValue)
            {
                entity.AreaID = result.Value;
                foreach (var gate in entity.Gates)
                {
                    Context.InsertAreaGateMapping(entity.AreaID, gate.GateID);
                }
                foreach (var app in entity.AreaApprovers)
                {
                    Context.InsertAreaApprover(entity.AreaID, app.Seq, app.DepartmentID, app.PositionID);
                }
            }
        }

        public override void Edit(Area entity)
        {
            // Update Area
            Context.UpdateArea(entity.AreaID, entity.FactoryCode, entity.AreaName, entity.AreaDisplayEN, entity.AreaDisplayTH, entity.ConfdtLevel, entity.IsActive, entity.UpdateBy);

            // Determine gates is modified
            if (entity.Gates.Any(t => String.IsNullOrEmpty(t.Name)))
            {
                // Clear Area Gate Mapping
                Context.DeleteAreaGateMapping(entity.AreaID);
               
                foreach (var gate in entity.Gates)
                {
                    // Insert Area Gate Mapping
                    Context.InsertAreaGateMapping(entity.AreaID, gate.GateID);
                }
            }
            //if (entity.AreaApprovers.Any(t => t.Seq == 0))
            //{
                Context.DeleteAreaApprover(entity.AreaID);
                foreach (var app in entity.AreaApprovers)
                {
                    Context.InsertAreaApprover(app.AreaID, app.Seq, app.DepartmentID, app.PositionID);
                }
            //}
        }

        public override void Remove(Area entity)
        {
            Context.DeleteAreaGateMapping(entity.AreaID);
            Context.DeleteAreaApprover(entity.AreaID);
            Context.DeleteArea(entity.AreaID);
        }


        public bool IsInUse(int AreaID)
        {
            var result = Context.CheckAreaInUse(AreaID).FirstOrDefault();


            if (result.HasValue)
            {
                return result.Value;
            }
            return false;


        }

        public bool IsDuplicate(Area entity)
        {
            var result = Context.CheckDuplicateArea(entity.AreaName, entity.FactoryCode).FirstOrDefault();
            if (result.HasValue)
            {
                return result.Value == 1;
            }
            return false;
        }

        public IEnumerable<AreaMapping> GetAreaMapping(string id)
        {
            return Context.GetAreaByEmployeeID(id).ToList();
        }
    }
}
