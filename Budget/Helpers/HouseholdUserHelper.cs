using Budget.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Budget.Helpers {
    public class HouseholdUserHelper {

        private ApplicationDbContext db = new ApplicationDbContext();

        public bool IsInHousehold(string memberId, int householdId) {

            if(db.HouseHoldData.Find(householdId).Users.Contains(db.Users.Find(memberId))) {
                return true;
                }
            return false;
            }

        public void AddMemberToHousehold(string memberId, int householdId) {
            if(!IsInHousehold(memberId, householdId)) {
            var household = db.HouseHoldData.Find(householdId);
                household.Users.Add(db.Users.Find(memberId));
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                }
            }

        public ICollection<ApplicationUser> ListMembersOfHousehold(int householdId) {
            return db.HouseHoldData.Find(householdId).Users;
            }

        }
    }