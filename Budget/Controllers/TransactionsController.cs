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
    public class TransactionsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        ApplicationUser Users = new ApplicationUser();

        // GET: Transactions
        public ActionResult Index() {
            var transactionData = db.TransactionData.Include(t => t.BankAccount).Include(t => t.Category).Include(t => t.User);
            
            return View(transactionData.ToList().Where(t => t.IsDeleted != true));
            }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id) {
            if(id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            Transaction transaction = db.TransactionData.Find(id);
            if(transaction == null) {
                return HttpNotFound();
                }
            return View(transaction);
            }

        // GET: Transactions/Create
        public ActionResult Create() {
            HouseHold households = new HouseHold();
            var user = db.Users.Find(User.Identity.GetUserId());
            //var household = db.HouseHoldData.Where(u => u.Id == user.HouseHoldId);
            var bankAccounts = db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId);

            ViewBag.BankAccountId = new SelectList(db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.CategoryData, "Id", "Name", "IsDeposit");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");

            return View();
            }

        // GET: Transactions/_Withdrawl
        public PartialViewResult _Withdrawl() {
            HouseHold households = new HouseHold();
            var user = db.Users.Find(User.Identity.GetUserId());
            //var household = db.HouseHoldData.Where(u => u.Id == user.HouseHoldId);
            var bankAccounts = db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId);

            ViewBag.BankAccountId = new SelectList(db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.CategoryData.Where(c => c.IsDeposit == false), "Id", "Name", "IsDeposit");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");

            return PartialView();
        }

        // GET: Transactions/_Withdrawl
        public PartialViewResult _Deposit() {
            HouseHold households = new HouseHold();
            var user = db.Users.Find(User.Identity.GetUserId());
            //var household = db.HouseHoldData.Where(u => u.Id == user.HouseHoldId);
            var bankAccounts = db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId);

            ViewBag.BankAccountId = new SelectList(db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId), "Id", "Name");
            ViewBag.CategoryId = new SelectList(db.CategoryData.Where(c => c.IsDeposit == true), "Id", "Name", "IsDeposit");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");

            return PartialView();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Withdrawl([Bind(Include = "Id,CategoryId,BankAccountId,IsDeleted,TransactionAmount,ReconsiliationAmount,TransactionDescription")] Transaction transaction) {

            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.HouseHoldData.Where(u => u.Id == user.HouseHoldId);
            var bankAccounts = db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId);

            if(ModelState.IsValid) {

            transaction.IsWithdrawl = true;
            transaction.TransactionAmount = transaction.TransactionAmount * -1;
            transaction.UserId = user.Id;
            transaction.DateCreated = DateTimeOffset.Now;

            db.TransactionData.Add(transaction);
            db.SaveChanges();   
            return RedirectToAction("Index");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccountData, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData.Where(c => c.IsDeposit == false), "Id", "Name", transaction.CategoryId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
            }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit([Bind(Include = "Id,CategoryId,BankAccountId,IsDeleted,TransactionAmount,ReconsiliationAmount,TransactionDescription")] Transaction transaction) {

            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.HouseHoldData.Where(u => u.Id == user.HouseHoldId);
            var bankAccounts = db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId);

            if(ModelState.IsValid) {

                transaction.IsWithdrawl = false;
                transaction.UserId = user.Id;
                transaction.DateCreated = DateTimeOffset.Now;

                db.TransactionData.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccountData, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData.Where(c => c.IsDeposit == true), "Id", "Name", transaction.CategoryId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id) {
            if(id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            Transaction transaction = db.TransactionData.Find(id);
            if(transaction == null) {
                return HttpNotFound();
                }
            ViewBag.BankAccountId = new SelectList(db.BankAccountData, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData, "Id", "Name", transaction.CategoryId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
            }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CategoryId,BankAccountId,UserId,IsWithdrawl,IsDeleted,DateCreated,DateEdited,TransactionAmount,ReconsiliationAmount,TransactionDescription")] Transaction transaction) {
            if(ModelState.IsValid) {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
                }
            ViewBag.BankAccountId = new SelectList(db.BankAccountData, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData, "Id", "Name", transaction.CategoryId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
            }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id) {
            if(id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            Transaction transaction = db.TransactionData.Find(id);

            if(transaction == null) {
                return HttpNotFound();
                }
            return View(transaction);
            }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) {
            Transaction transaction = db.TransactionData.Find(id);
            //db.TransactionData.Remove(transaction);
            transaction.IsDeleted = true;
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
