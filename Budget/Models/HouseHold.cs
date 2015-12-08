using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class HouseHold {

        public HouseHold() {
            this.Users = new HashSet<ApplicationUser>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.Categories = new HashSet<Category>();
            this.BudgetItems = new HashSet<BudgetItems>();
        }

        public int Id { get; set; }

        [Display(Name = "Household Name")]
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<BudgetItems> BudgetItems { get; set; }

    }
}