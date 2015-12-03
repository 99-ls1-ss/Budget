using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Dashboard {

        //public TicketsModel() {
        //    this.Attachments = new HashSet<TicketAttachmentsModel>();

        public Dashboard() {
            this.Transactions = new HashSet<Transaction>();
            this.BankAccounts = new HashSet<BankAccount>();
            this.Members = new HashSet<Member>();
        }

        public virtual HouseHold Household { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Member> Members { get; set; }
    }
}