using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Transaction {

        public int Id { get; set; }
        public int? CategoryId { get; set; }
        public int? BankAccountId { get; set; }
        public string UserId { get; set; }

        public bool IsWithdrawl { get; set; }
        public bool IsDeleted { get; set; }

        [Display(Name = "Date Created")]
        [DisplayFormat(DataFormatString = "{0:M/dd/yyyy hh:mmtt}")]
        public DateTimeOffset DateCreated { get; set; }
        
        [Display(Name = "Date Updated")]
        [DisplayFormat(DataFormatString = "{0:M/dd/yyyy hh:mmtt}")]
        public Nullable<DateTimeOffset> DateEdited { get; set; }

        [Display(Name = "Transaction Amount")]
        public decimal TransactionAmount { get; set; }

        [Display(Name = "Reconsiliation Amount")]
        public decimal? ReconsiliationAmount { get; set; }

        [Display(Name = "Description")]
        public string TransactionDescription { get; set; }        

        public virtual Category Category { get; set; }
        public virtual BankAccount BankAccount { get; set; }
        public virtual ApplicationUser User { get; set; }        

    }
}