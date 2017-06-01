using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nalco_Consumables.Models
{
    public class Materials
    {
        public int id { get; set; }
        public string materialcode { get; set; }
        public string materialdescription { get; set; }
        public string printerdescription { get; set; }
        public int printercount { get; set; }
        public int quantity { get; set; }
        public bool criticalflag { get; set; }
        public int reorderleve { get; set; }
        public bool centralstorage { get; set; }
    }
}