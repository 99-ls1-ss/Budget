using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class BudgetItems {

        public int Id { get; set; }
        [Display(Name = "Budget Item Name")]
        public string Name { get; set; }
        [Display(Name = "Budgeted Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Household Id")]
        public int? HouseHoldId { get; set; }
        [Display(Name = "Category Id")]
        public int? CategoryId { get; set; }

        [Display(Name = "Household Name")]
        public virtual HouseHold HouseHold { get; set; }
        [Display(Name = "Category")]
        public virtual Category Category { get; set; }

    }
}