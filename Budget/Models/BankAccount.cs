﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class BankAccount {

        public BankAccount() {
            this.Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        [Display(Name = "Bank Account Name")]
        public string Name { get; set; }
        public int? HouseHoldId { get; set; }
        public decimal Balance { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual HouseHold HouseHold { get; set; }
    }
}