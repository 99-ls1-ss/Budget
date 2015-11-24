using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class HouseHold {

        public HouseHold() {
            this.Users = new HashSet<ApplicationUser>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.BudgetItems = new HashSet<Budget>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Budget> BudgetItems { get; set; }

    }
}