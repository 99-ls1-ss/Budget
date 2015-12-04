using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class TransactionByCategoryVM {

        public virtual BankAccount BankAccount { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual Category Category { get; set; }
        public List<Category> Categories {get; set;}
        public List<Transaction> Transactions { get; set; }

    }
}