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
        public ActionResult Index(int? id, string sentCode, string inviteEmail) {

            var memberInvitation = db.MemberData.Where(g => g.Email == inviteEmail).FirstOrDefault();
            var user = db.Users.Find(User.Identity.GetUserId());           
            //var id = user.HouseHoldId;
            HouseHold household = db.HouseHoldData.Find(id);

            if(!Request.IsAuthenticated) {
                return RedirectToAction("Login", "Account");
            } else if(user.HouseHoldId == null) {
                return RedirectToAction("Create");
            } else if(memberInvitation != null && memberInvitation.GUID == sentCode && memberInvitation.IsRegistered != true) {
                return RedirectToAction("JoinHousehold");
            } else {
                return RedirectToAction("Details", new { id = user.HouseHoldId });
            }
        }

        // GET: HouseHolds/Details/5
        public ActionResult Details(int? id) {

            var user = db.Users.Find(User.Identity.GetUserId());
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
            var callbackUrl = Url.Action("JoinHousehold", "HouseHolds", new { id = household.Id, sentCode = code, inviteEmail = inviteEmail}, protocol: Request.Url.Scheme);
            
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
        [Authorize]
        public ActionResult JoinHousehold(int? id, string sentCode, string inviteEmail) {

            var memberInvitation = db.MemberData.Where(g => g.Email == inviteEmail && g.IsRegistered == false).FirstOrDefault();

            if(memberInvitation == null) {
                return RedirectToAction("Create");
            } else if(memberInvitation.GUID == sentCode) {
                var user = db.Users.Find(User.Identity.GetUserId());
                user.HouseHoldId = id;
                memberInvitation.IsRegistered = true;
                db.SaveChanges();
                return RedirectToAction("Index", "BankAccounts");
            }

            return RedirectToAction("Create");
        }

        [HttpPost]
        [AuthorizeHouseholdRequired]
        public async Task<ActionResult> LeaveHousehold() {
            var user = db.Users.Find(User.Identity.GetUserId());
            user.HouseHoldId = null;
            db.SaveChanges();
            await ControllerContext.HttpContext.RefreshAuthentication(user);
            return RedirectToAction("Create");
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

                if(user.HouseHoldId == null) {
                    db.HouseHoldData.Add(houseHold);
                    user.HouseHoldId = houseHold.Id;
                    db.SaveChanges();
                }

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
