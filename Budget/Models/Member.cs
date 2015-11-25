using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Budget.Models {
    public class Member {

        public int Id { get; set; }
        [Display(Name = "Member Email")]
        public string Email { get; set; }
        public string GUID { get; set; }
        public bool IsRegistered { get; set; }
        public int? HouseHoldId { get; set; }

        public virtual HouseHold HouseHold { get; set; }

    }
}