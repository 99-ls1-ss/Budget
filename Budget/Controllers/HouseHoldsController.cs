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
using Newtonsoft.Json;

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
            HouseHold household = db.HouseHoldData.Find(id);
            if(household == null) {
                return HttpNotFound();
            }

            return View(household);
        }

        // GET Partial: HouseHolds/_Members 
        public PartialViewResult _Members(int? id) {
            var user = db.Users.Find(User.Identity.GetUserId());
            HouseHold household = db.HouseHoldData.Find(id);
            return PartialView();
        }

        // GET Partial: HouseHolds/_BankAccounts 
        public PartialViewResult _BankAccounts(int? id) {
            var user = db.Users.Find(User.Identity.GetUserId());
            BankAccount bankAccounts = db.BankAccountData.Find(id);
            HouseHold household = db.HouseHoldData.Find(id);
            return PartialView(household);
        }

        // GET Dashboard: HouseHolds/Dashboard
        public ActionResult Dashboard() { 

            var user = db.Users.Find(User.Identity.GetUserId());
            BankAccount bankAccounts = db.BankAccountData.Find(user.HouseHoldId);
            Transaction transactions = db.TransactionData.Find(bankAccounts.HouseHoldId);
            HouseHold household = db.HouseHoldData.Find(user.HouseHoldId);

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

        public ActionResult GetChart() {
            ApplicationDbContext db = new ApplicationDbContext();
            Transaction transaction = new Transaction();
            HouseHold household = db.HouseHoldData.Find(Convert.ToInt32(User.Identity.GetHouseholdid()));
            var withdrawlChartArray = (from c in household.Categories
                              where  c.IsDeposit == false
                              let sum = (from b in household.BudgetItems
                                         where b.CategoryId == c.Id
                                         select b.Amount).DefaultIfEmpty().Sum()
                              select new {
                                  label = c.Name,
                                  data = sum

                              }).ToArray();

            var depositChartArray = (from c in household.Categories
                              where c.IsDeposit == true
                              let sum = (from b in household.BudgetItems
                                         where b.CategoryId == c.Id
                                         select b.Amount).DefaultIfEmpty().Sum()
                              select new {
                                  label = c.Name,
                                  data = sum

                              }).ToArray();

            var jsonData = new {
                Expense = withdrawlChartArray,
                Income = depositChartArray
            };

            return Content(JsonConvert.SerializeObject(jsonData), "application/json");

        }

        public ActionResult GetMonthly() {
            var hh = db.HouseHoldData.Find(Convert.ToInt32(User.Identity.GetHouseholdid()));
            var budgetName = db.BudgetData.Find(Convert.ToInt32(User.Identity.GetHouseholdid()));
            var monthToDate = Enumerable.Range(1, DateTime.Today.Month)
                .Select(m => new DateTime(DateTime.Today.Year, m, 1))
                .ToList();

            var sums  = (from month in monthToDate
                        select new {
                            month = month.ToString("MMM"),
                            actualExpense = (from account in hh.BankAccounts
                                             from transaction in account.Transactions
                                             where transaction.DateCreated.Month == month.Month
                                             select (transaction.TransactionAmount)).DefaultIfEmpty().Sum(),
                            
                            budgetExpense = (from budget in hh.BudgetItems
                                             where budget.Category.IsDeposit == true
                                             select (budget.Amount)).DefaultIfEmpty().Sum()
                            
                        }).ToArray();

            return Content(JsonConvert.SerializeObject(sums), "application/json");
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
