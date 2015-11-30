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

            var code = Guid.NewGuid().ToString("n");
            var callbackUrl = Url.Action("JoinHousehold", "HouseHolds", new { id=household.Id, sentCode = code, inviteEmail = inviteEmail}, protocol: Request.Url.Scheme);
            //var callbackUrl = Url.Action("Login", "Account", null, protocol: Request.Url.Scheme);
            
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


        // GET: HouseHolds/JoinHousehold
        public ActionResult JoinHousehold(int? id, string sentCode, string inviteEmail) {

            ApplicationUser user = new ApplicationUser();
            var guid = db.MemberData.Where(g => g.GUID == sentCode).FirstOrDefault();

            return View();
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
                var user = db.Users.Find(User.Identity.GetUserId()); 
                db.HouseHoldData.Add(houseHold);
                db.SaveChanges();
                //TODO

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
