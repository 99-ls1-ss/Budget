using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Budget.Models;
using Microsoft.AspNet.Identity;

namespace Budget.Controllers {
    [AuthorizeHouseholdRequired]
    public class BankAccountsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BankAccounts
        public ActionResult Index(HouseHold household) {

            var user = db.Users.Find(User.Identity.GetUserId());
            var bankAccountData = db.BankAccountData.Include(b => b.HouseHold);
            var myAccounts = db.BankAccountData.Where(a => a.HouseHoldId == user.HouseHoldId).ToList();
            
            if(bankAccountData != null){
                return RedirectToAction("Details", new { id = user.HouseHoldId});
            }

            return RedirectToAction("Create");
        }

        // GET: BankAccounts/Details/5
        public ActionResult Details(int? bankAccountId, int? householdId) {

            BankAccount bankAccount = db.BankAccountData.Find(bankAccountId);

            var user = db.Users.Find(User.Identity.GetUserId());

            var myAccounts = db.BankAccountData.Where(a => a.HouseHoldId == user.HouseHoldId).ToList();
            
            if(bankAccount == null) {
                return RedirectToAction("Create");
            }
            return View(myAccounts);
        }

        // GET: BankAccounts/Create
        public ActionResult Create() {
            ViewBag.HouseHoldId = new SelectList(db.HouseHoldData, "Id", "Name");
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,HouseHoldId,Balance")] BankAccount bankAccount) {

            var user = db.Users.Find(User.Identity.GetUserId());

            //if(ModelState.IsValid) {
                db.BankAccountData.Add(bankAccount);
                bankAccount.HouseHoldId = user.HouseHoldId;
                db.SaveChanges();
                return RedirectToAction("Index", "HouseHolds", new { id = user.HouseHoldId });
            //}

            //ViewBag.HouseHoldId = new SelectList(db.HouseHoldData, "Id", "Name", bankAccount.HouseHoldId);
            //return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id) {

            var user = db.Users.Find(User.Identity.GetUserId());
            var householdId = db.BankAccountData.Where(h => h.HouseHoldId == user.HouseHoldId);
            BankAccount bankAccount = db.BankAccountData.Find(id);

            if(householdId == null) {
                return RedirectToAction("Create");
            }
            ViewBag.HouseHoldId = new SelectList(db.HouseHoldData, "Id", "Name", bankAccount.HouseHoldId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,HouseHoldId,Balance")] BankAccount bankAccount) {
            if(ModelState.IsValid) {
                db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseHoldId = new SelectList(db.HouseHoldData, "Id", "Name", bankAccount.HouseHoldId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public ActionResult Delete(int? id) {
            if(id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BankAccount bankAccount = db.BankAccountData.Find(id);
            if(bankAccount == null) {
                return HttpNotFound();
            }
            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            BankAccount bankAccount = db.BankAccountData.Find(id);
            db.BankAccountData.Remove(bankAccount);
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