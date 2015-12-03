using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class TransactionByCategoryVM {

        public List<Category> Categories {get; set;}
        public List<Transaction> Transactions { get; set; }

    }
}