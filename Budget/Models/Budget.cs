using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Budget {

        public int Id { get; set; }
        public decimal Amount { get; set; }
        public int? HouseHoldId { get; set; }
        public int? CategoryId { get; set; }

        public virtual HouseHold HouseHold { get; set; }
        public virtual Category Category { get; set; }

    }
}