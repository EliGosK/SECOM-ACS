using CSI.Web.Mvc;
using SECOM.ACS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SECOM.ACS.MvcWebApp.Models
{
    [ModelBinder(typeof(CardViewModelBinder))]
    public class CardViewModel
    {
        [Required]
        [StringLength(50)]
        public string CardID { get; set; }

        [Required]
        [StringLength(50)]
        public string CardType { get; set; }

        [Required]
        [StringLength(50)]
        public string CardNo { get; set; }

        public string CreateBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string UpdateBy { get; set; }
        public System.DateTime UpdateDate { get; set; }
        public bool EditAble { get; set; }
        public bool DeleteAble { get; set; }
        /// <summary>
        /// IsActive
        /// </summary>
        public string IsActive { get; set; }
        [StringLength(256)]
        public string Note { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

  
        [Required]
        public IList<AreaDataViewModel> Area { get; private set; } = new List<AreaDataViewModel>();

        public int[] SelectedArea { get; set; }



    }

    public class CardViewModelBinder : ExtendedModelBinder
    {
        public override object BindModel(ControllerContext controllerContext,
                            ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext) as CardViewModel;
            if (model != null)
            {
                if (model.CardType == CardType.VIP)
                {
                    var q = from a in model.Area
                            join sa in model.SelectedArea
                            on a.AreaID equals sa
                            select a;

                    if (q.Count() != model.Area.Count || q.Count() != model.SelectedArea.Length)
                    {
                        model.Area.Clear();
                        foreach (var areaId in model.SelectedArea)
                        {
                            model.Area.Add(new AreaDataViewModel() { AreaID = areaId });
                        }
                    }
                }
            }
            return model;

        }
    }
}