using CSI.ComponentModel;
using Newtonsoft.Json;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Tasks
{
    public class EmployeeImportData
    {
        public string EmpID { get; set; }
        public string CardID { get; set; }
        public string Gender { get; set; }
        public string EmpNameTH { get; set; }
        public string EmpNameEN { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Level { get; set; }
        public string SpecialPosition { get; set; }
        public DateTime StartWorkingDate { get; set; }
        public DateTime? ResignDate { get; set; }

        public string Area1 { get; set; }
        public string Area2 { get; set; }
        public string Area3 { get; set; }
        public string Area4 { get; set; }
        public string Area5 { get; set; }
        public string Area6 { get; set; }
        public string Area7 { get; set; }
        public string Area8 { get; set; }
        public string Area9 { get; set; }
        public string Area10 { get; set; }

        public string Area11 { get; set; }
        public string Area12 { get; set; }
        public string Area13 { get; set; }
        public string Area14 { get; set; }
        public string Area15 { get; set; }
        public string Area16 { get; set; }
        public string Area17 { get; set; }
        public string Area18 { get; set; }
        public string Area19 { get; set; }
        public string Area20 { get; set; }

        public string Area21 { get; set; }
        public string Area22 { get; set; }
        public string Area23 { get; set; }
        public string Area24 { get; set; }
        public string Area25 { get; set; }
        public string Area26 { get; set; }
        public string Area27 { get; set; }
        public string Area28 { get; set; }
        public string Area29 { get; set; }
        public string Area30 { get; set; }

        public string Area31 { get; set; }
        public string Area32 { get; set; }
        public string Area33 { get; set; }
        public string Area34 { get; set; }
        public string Area35 { get; set; }
        public string Area36 { get; set; }
        public string Area37 { get; set; }
        public string Area38 { get; set; }
        public string Area39 { get; set; }
        public string Area40 { get; set; }

        public string Area41 { get; set; }
        public string Area42 { get; set; }
        public string Area43 { get; set; }
        public string Area44 { get; set; }
        public string Area45 { get; set; }
        public string Area46 { get; set; }
        public string Area47 { get; set; }
        public string Area48 { get; set; }
        public string Area49 { get; set; }
        public string Area50 { get; set; }

        [JsonIgnore]
        public List<Exception> Errors { get; private set; } = new List<Exception>();
        [JsonIgnore]
        public Position PositionMaster { get; set; }
        [JsonIgnore]
        public Position SpecialPositionMaster { get; set; }
        [JsonIgnore]
        public Department DepartmentMaster { get; set; }

        public bool IsValid { get { return this.Errors.Count==0; } }

        /// <summary>
        /// Ensure trimming data before perform data validation.
        /// </summary>
        public void EnsureTrimmingString()
        {
            foreach (var p in typeof(EmployeeImportData).GetProperties())
            {
                if (p.PropertyType != typeof(string)) {
                    continue;
                }
                var value = (string)p.GetValue(this, null);
                p.SetValue(this, String.IsNullOrEmpty(value) ? String.Empty : value.Trim());
            } 
            //this.EmpID = String.IsNullOrEmpty(this.EmpID) ? "" : this.EmpID.Trim();
            //this.CardID = String.IsNullOrEmpty(this.EmpID) ? "" : this.CardID.Trim();
            //this.Department = String.IsNullOrEmpty(this.Department) ? "" : this.Department.Trim();
            //this.Position = String.IsNullOrEmpty(this.Position) ? "" : this.Position.Trim();
            //this.SpecialPosition = String.IsNullOrEmpty(this.SpecialPosition) ? "" : this.SpecialPosition.Trim();

            //for (int i = 1; i <= 50; i++)
            //{
            //    var propertyName = $"Area{i}";
            //    var p = typeof(EmployeeImportData).GetProperty(propertyName);
            //    if (p == null) { continue; }
            //    var areaName = (string)p.GetValue(this, null);
            //    p.SetValue(this, String.IsNullOrEmpty(areaName) ? "" : areaName.Trim());
            //}
        }
        

        public void AddError(string message)
        {
            Errors.Add(new Exception(message));
        }

        public void AddError(Exception ex)
        {
            this.Errors.Add(ex);
        }

        public ObjectResults<Area> ValidateArea(Area[] areaList)
        {
            var results = new ObjectResults<Area>();
            for (int i = 1; i <= 50; i++)
            {
                var propertyName = $"Area{i}";
                var p = typeof(EmployeeImportData).GetProperty(propertyName);
                if (p == null) { continue; }

                var areaName = (string)p.GetValue(this, null);
                if (String.IsNullOrEmpty(areaName)) { continue; }
                var area = areaList.FirstOrDefault(t => String.Compare(areaName, t.AreaDisplayEN, true) == 0);
                if (area == null)
                {
                    results.AddResult(new Area() { AreaDisplayEN = areaName }, new Exception($"Invalid area name in column {p.Name}. Area name: {areaName} is not found in area master data. "));
                }
                else {
                    results.AddResult(area);
                }
                
            }
            return results;
        }


        public Employee ToEmployee(string user, Area[] areaList)
        {
            var employee = new Employee()
            {
                EmpID = this.EmpID,
                CardID = this.CardID,
                CreateBy = user,
                CreateDate = DateTime.Now,
                Email = this.Email,
                DepartmentID = this.DepartmentMaster.DepartmentID,
                PositionID = this.PositionMaster.PositionID,
                SpecialPositionID = this.SpecialPositionMaster == null?null: this.SpecialPositionMaster.PositionID,
                Level = this.Level,
                UpdateBy = user,
                UpdateDate = DateTime.Now,
                EmpNameEN = this.EmpNameEN,
                EmpNameTH = this.EmpNameTH,
                IsHRLink = true,
                IsShow = true,
                IsActive = true,
                StartWorkingDate = this.StartWorkingDate,
                ResignDate = this.ResignDate
            };

            var results = this.ValidateArea(areaList);
            results.GetSuccesses().Select(t => t.AreaID)
                .ToList()
                .ForEach((int areaId) => employee.AreaMappings.Add(new AreaMapping() {
                    EmpID = this.EmpID,
                    CardID = this.CardID,
                    AreaID = areaId,
                    IsMainArea = employee.AreaMappings.Count()==0
                }));
            
            return employee;
        }

        public Card ToCard(string user)
        {
            return new Card()
            {
                CardID = this.CardID,
                CardNo = this.EmpID,
                CardType = CardType.Employee,
                CreateBy = user,
                CreateDate = DateTime.Now,
                UpdateBy = user,
                UpdateDate = DateTime.Now,
                DeleteAble = false,
                EditAble = false,
                IsActive = true,
                Note = "Created by from ACP020"
            };
        }

        public AreaOrganizeSearchCriteria ToAreaOrganizeCriteria()
        {
            return new AreaOrganizeSearchCriteria()
            {
                DepartmentID = this.DepartmentMaster==null? null: this.DepartmentMaster.DepartmentID,
                PositionID = this.PositionMaster==null? null : this.PositionMaster.PositionID,
                SpecialPositionID = this.SpecialPositionMaster == null ? null : this.SpecialPositionMaster.PositionID,
                Level = this.Level
            };
        }
    }
}
