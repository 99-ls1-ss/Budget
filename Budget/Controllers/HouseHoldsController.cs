using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budget.Models;
using Budget.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace Budget.Controllers {
    [Authorize]
    public class HouseHoldsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HouseHold
        
        public ActionResult Index() {

            var user = db.Users.Find(User.Identity.GetUserId());           
            var id = user.HouseHoldId;
            HouseHold household = db.HouseHoldData.Find(id);

            if(!Request.IsAuthenticated) {
                return RedirectToAction("Login", "Account");
            }

            if(id != null) {            
                return View(household);
            }
            else {
                return RedirectToAction("Create");
            }
        }

        // GET: HouseHolds/Details/5
        public ActionResult Details(int? id) {

            //var user = db.Users.Find(User.Identity.GetUserId());
            //var id = user.HouseHoldId;
            HouseHold household = db.HouseHoldData.Find(id);
            if(household == null) {
                return HttpNotFound();
            }

            return View(household);
        }

        // GET: HouseHolds/Invitation
        [HttpPost]
        public ActionResult Invite(HouseHold household, string inviteEmail) {

            //var callbackUrl = Url.Action("JoinHousehold", "Details", null, protocol: Request.Url.Scheme);
            var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme);
            var code = Guid.NewGuid().ToString("n");
            EmailService es = new EmailService();
            IdentityMessage im = new IdentityMessage() {
                Destination = inviteEmail,
                Subject = "You have been invited to join the " + household.Name + ".",
                Body = "You can join the " + household.Name + " by clicking the following link. <br />" + callbackUrl + "<br /> Verification Code:" + code
            };
            es.SendAsync(im);
            var user = db.Users.Find(User.Identity.GetUserId());
            Member m = new Member() {
                GUID = code,
                Email = inviteEmail,
                HouseHoldId = user.HouseHoldId
            };
            db.MemberData.Add(m);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = household.Id });
        }

        [HttpPost]
        public ActionResult JoinHousehold(int householdId, string inviteEmail, string sentCode) {

            var user = db.Users.Find(User.Identity.GetUserId());
            var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme);
            var member = db.MemberData.Where(m => m.Email == inviteEmail).FirstOrDefault();
            var memberCode = db.MemberData.Where(c => c.GUID == sentCode).FirstOrDefault();
            var household = db.MemberData.Where(h => h.HouseHoldId == householdId).FirstOrDefault();

            if(sentCode == memberCode.GUID && inviteEmail == memberCode.Email && householdId == memberCode.HouseHoldId) {
                user.HouseHoldId = householdId;
                db.SaveChanges();
                return RedirectToAction("Details", new { id = householdId });
            }
            else {
                return RedirectToAction("Login", "Account");
            }
            
        }

        // GET: HouseHolds/Create
        public ActionResult Create() {
            return View();
        }

        // POST: HouseHolds/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name")] HouseHold houseHold) {
            if(ModelState.IsValid) {
                db.HouseHoldData.Add(houseHold);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(houseHold);
        }

        // GET: HouseHolds/Edit/5
        public ActionResult Edit(int? id) {
            if(id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var householdId = db.HouseHoldData.Find(id);
            var selected = householdId.Users.Select(u => u.Id);

            HouseholdUser householdMembers = new HouseholdUser() {
                Members = new MultiSelectList(db.Users, "Id", "DisplayName", selected),
                HouseHolds = householdId
            };

            return View(householdMembers);
        }


        // POST: HouseHolds/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HouseholdUser householdUser) {

            var household = db.HouseHoldData.Find(householdUser.HouseHolds.Id);

            HouseholdUserHelper helper = new HouseholdUserHelper();
            household.Users.Clear();

            foreach(var memberid in householdUser.SelectedMembers) {
                helper.AddMemberToHousehold(memberid, household.Id);
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // GET: HouseHolds/Delete/5
        public ActionResult Delete(int? id) {
            if(id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HouseHold houseHold = db.HouseHoldData.Find(id);
            if(houseHold == null) {
                return HttpNotFound();
            }
            return View(houseHold);
        }

        // POST: HouseHolds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            HouseHold houseHold = db.HouseHoldData.Find(id);
            db.HouseHoldData.Remove(houseHold);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing) {
            if(disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
