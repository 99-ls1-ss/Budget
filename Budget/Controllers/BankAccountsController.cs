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
        public ActionResult Details(int? bankAccountId) {

            var user = db.Users.Find(User.Identity.GetUserId());
            var householdId = db.BankAccountData.Where(h => h.HouseHoldId == user.HouseHoldId);
            BankAccount bankAccount = db.BankAccountData.Find(bankAccountId);
            Transaction transactions = db.TransactionData.Find(bankAccountId);
            var myAccounts = db.TransactionData.Where(a => a.BankAccountId == bankAccount.Id);

            if(transactions.IsDeleted != false) {
                var accountBalance = db.TransactionData.Where(t => t.BankAccountId == bankAccount.Id && t.IsDeleted == false).Select(a => a.TransactionAmount).Sum();
            }
            else {
                var accountBalance = db.TransactionData.Where(t => t.BankAccountId == bankAccount.Id && t.IsDeleted == false).Select(a => a.TransactionAmount);
            }

            return View(bankAccount);
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

            db.BankAccountData.Add(bankAccount);
            bankAccount.HouseHoldId = user.HouseHoldId;
            db.SaveChanges();
            return RedirectToAction("Index", "HouseHolds", new { id = user.HouseHoldId });

        }

        // GET: BankAccounts/Edit/5
        public ActionResult Edit(int? id) {

            var user = db.Users.Find(User.Identity.GetUserId());
            var householdId = db.BankAccountData.Where(h => h.HouseHoldId == user.HouseHoldId);
            BankAccount bankAccount = db.BankAccountData.Find(id);
            Transaction transactions = db.TransactionData.Find(id);
            var myAccounts = db.TransactionData.Where(a => a.BankAccountId == bankAccount.Id);

            if(transactions.IsDeleted != false) {
                var accountBalance = db.TransactionData.Where(t => t.BankAccountId == bankAccount.Id && t.IsDeleted == false).Select(a => a.TransactionAmount).Sum();
            }
            else {
                var accountBalance = db.TransactionData.Where(t => t.BankAccountId == bankAccount.Id && t.IsDeleted == false).Select(a => a.TransactionAmount);
            }

            //var accountBalance = db.TransactionData.Where(t => t.BankAccountId == myAccounts.Id && t.IsDeleted == false).Select(a => a.TransactionAmount).Sum();

            ViewBag.HouseHoldId = new SelectList(db.HouseHoldData, "Id", "Name", bankAccount.HouseHoldId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BankAccount bankAccount) {
            var user = db.Users.Find(User.Identity.GetUserId());
                
                db.BankAccountData.Attach(bankAccount);
                db.Entry(bankAccount).Property(p => p.Name).IsModified = true;
                db.Entry(bankAccount).Property(p => p.HouseHoldId).IsModified = true;
                db.Entry(bankAccount).Property(p => p.Balance).IsModified = true;

                //db.Entry(bankAccount).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
            
            //ViewBag.HouseHoldId = new SelectList(db.HouseHoldData, "Id", "Name", bankAccount.HouseHoldId);
            return RedirectToAction("Index", "HouseHolds", new { id = user.HouseHoldId });
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
            var user = db.Users.Find(User.Identity.GetUserId());
            BankAccount bankAccount = db.BankAccountData.Find(id);
            db.BankAccountData.Remove(bankAccount);
            db.SaveChanges();
            return RedirectToAction("Index", "Households", new { id = user.HouseHoldId });
        }

        protected override void Dispose(bool disposing) {
            if(disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}