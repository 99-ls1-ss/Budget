using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budget.Models {
    public class HouseholdUser {

        public HouseHold HouseHolds { get; set; }
        public MultiSelectList Members { get; set; }

        public string[] SelectedMembers { get; set; }
        public string[] UserHouseholds { get; set; }

        }
    }