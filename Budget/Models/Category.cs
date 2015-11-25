using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Category {

        public Category() {
            this.Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        public virtual ICollection<Transaction> Transactions {get; set;}

    }
}