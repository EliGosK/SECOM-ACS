using CSI.ComponentModel;
using CSI.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SECOM.ACS.Models
{
    public partial class Card
    {
       
    }

    public class CardSearchCriteria
    {
        public string CardNo { get; set; }
        public string[] CardType { get; set; }
        public string[] Status { get; set; }

        public string StatusValue
        {
            get
            {
                if (this.Status != null && this.Status.Length > 0)
                {
                    return String.Join(",", this.Status);
                }
                return null;
            }
        }

        public string CardTypeValue
        {
            get
            {
                if (this.CardType != null && this.CardType.Length > 0)
                {
                    return String.Join(",", this.CardType);
                }
                return null;
            }
        }

        public void EnsureDataValid()
        {
            
        }
    }

    public class ImportCardResult : ObjectResultDataItem
    {
        public Card Card { get; set; }

        public ImportCardResult(Card entity) : base()
        {
            this.Card = entity;
        }

        public ImportCardResult(Card entity,Exception ex) : base(ex)
        {
            this.Card = entity;
        }
    }



}
