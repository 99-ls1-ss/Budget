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


        // GET: Transactions/_Transactions
        public PartialViewResult _Transactions(int? bankAccountId) {
            var user = db.Users.Find(User.Identity.GetUserId());
            BankAccount bankAccounts = db.BankAccountData.Find(bankAccountId);
            Transaction transactions = new Transaction();
            var isEdited = bankAccounts.Transactions.Where(e => e.DateEdited != null);
            var transaction = bankAccounts.Transactions.Where(t => t.IsDeleted == false).ToList().OrderByDescending(o => o.DateCreated);
            var transactionEdited = bankAccounts.Transactions.Where(t => t.IsDeleted == false).ToList().OrderByDescending(o => o.DateEdited);

            if(transactions.DateEdited != null) {
                return PartialView(transactionEdited);
            }
            else {
                return PartialView(transaction);
            }

            //return PartialView(transaction);
        }


        // GET: Transactions/_CatTransactions
        public PartialViewResult _CatTransactions() {

            TransactionByCategoryVM vm = new TransactionByCategoryVM();
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.HouseHoldData.Find(Convert.ToInt32(User.Identity.GetHouseholdid()));
            
            vm.Categories = db.CategoryData.ToList();
            vm.Transactions = household.BankAccounts.SelectMany(b => b.Transactions).Where(t => t.IsDeleted == false).OrderByDescending(o => o.DateCreated).ToList();

            return PartialView(vm);
        }


        // GET: Transactions/_WithdrawlTransactions
        public PartialViewResult _WithdrawlTransactions() {

            TransactionByCategoryVM incomeExpense = new TransactionByCategoryVM();
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.HouseHoldData.Find(Convert.ToInt32(User.Identity.GetHouseholdid()));

            incomeExpense.Categories = db.CategoryData.ToList();
            incomeExpense.Transactions = household.BankAccounts.SelectMany(b => b.Transactions).Where(t => t.IsDeleted == false).OrderByDescending(o => o.DateCreated).ToList();

            return PartialView(incomeExpense);
        }

        // GET: Transactions/_IncomeTransactions
        public PartialViewResult _IncomeTransactions() {

            TransactionByCategoryVM incomeExpense = new TransactionByCategoryVM();
            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.HouseHoldData.Find(Convert.ToInt32(User.Identity.GetHouseholdid()));

            incomeExpense.Categories = db.CategoryData.ToList();
            incomeExpense.Transactions = household.BankAccounts.SelectMany(b => b.Transactions).Where(t => t.IsDeleted == false).OrderByDescending(o => o.DateCreated).ToList();

            return PartialView(incomeExpense);
        }


        // GET: Transactions/Create
        public ActionResult Create(int? bankAccountId) {
            HouseHold households = new HouseHold();
            var user = db.Users.Find(User.Identity.GetUserId());
            BankAccount accounts = db.BankAccountData.Single(b => b.Id == bankAccountId);            
            var bankAccounts = db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId);  
            var thisAccount = db.BankAccountData.Where(h => h.Id == bankAccountId);

            ViewBag.BankAccountId = new SelectList(db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId), "Id", "Name");
            ViewBag.BankAccount = db.BankAccountData.Single(ba => ba.Id == bankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData, "Id", "Name", "IsDeposit");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");

            return View();
            }

        // GET: Transactions/_Withdrawl
        public PartialViewResult _Withdrawl(int? bankAccountId) {
            HouseHold households = new HouseHold();
            BankAccount accounts = db.BankAccountData.Single(b => b.Id == bankAccountId);

            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.HouseHoldData.Where(u => u.Id == user.HouseHoldId);
            var bankAccounts = db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId);

            ViewBag.BankAccountId = new SelectList(db.BankAccountData.Where(b => b.Id == bankAccountId), "Id", "Name");
            ViewBag.BankAccount = db.BankAccountData.Single(ba => ba.Id == bankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData.Where(c => c.IsDeposit == false), "Id", "Name", "IsDeposit");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");

            return PartialView();
        }

        // GET: Transactions/_Deposit
        public PartialViewResult _Deposit(int? bankAccountId) {
            HouseHold households = new HouseHold();
            BankAccount accounts = db.BankAccountData.Single(b => b.Id == bankAccountId);

            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.HouseHoldData.Where(u => u.Id == user.HouseHoldId);
            var bankAccounts = db.BankAccountData.Where(b => b.HouseHoldId == user.HouseHoldId);

            ViewBag.BankAccountId = new SelectList(db.BankAccountData.Where(b => b.Id == bankAccountId), "Id", "Name");
            ViewBag.BankAccount = db.BankAccountData.Single(ba => ba.Id == bankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData.Where(c => c.IsDeposit == true), "Id", "Name", "IsDeposit");
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");

            return PartialView();
        }

        // POST: Transactions/Withdrawl
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Withdrawl([Bind(Include = "Id,CategoryId,BankAccountId,IsDeleted,TransactionAmount,ReconsiliationAmount,TransactionDescription")] Transaction transaction) {

            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.HouseHoldData.Where(u => u.Id == user.HouseHoldId);
            var bankAccount = db.BankAccountData.Single(b => b.Id == transaction.BankAccountId);

            if(ModelState.IsValid) {
                transaction.IsWithdrawl = true;
                transaction.TransactionAmount = transaction.TransactionAmount * -1;
                transaction.UserId = user.Id;
                transaction.DateCreated = DateTimeOffset.Now;

                bankAccount.Balance = bankAccount.Balance + transaction.TransactionAmount;

                db.TransactionData.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index", "Households");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccountData, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData.Where(c => c.IsDeposit == false), "Id", "Name", transaction.CategoryId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
            }

        // POST: Transactions/Deposit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Deposit([Bind(Include = "Id,CategoryId,BankAccountId,IsDeleted,TransactionAmount,ReconsiliationAmount,TransactionDescription")] Transaction transaction) {

            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.HouseHoldData.Where(u => u.Id == user.HouseHoldId);
            var bankAccount = db.BankAccountData.Single(b => b.Id == transaction.BankAccountId);

            if(ModelState.IsValid) {

                transaction.IsWithdrawl = false;
                transaction.UserId = user.Id;
                transaction.DateCreated = DateTimeOffset.Now;

                bankAccount.Balance = bankAccount.Balance + transaction.TransactionAmount;

                db.TransactionData.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index","Households");
            }

            ViewBag.BankAccountId = new SelectList(db.BankAccountData, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData.Where(c => c.IsDeposit == true), "Id", "Name", transaction.CategoryId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
        }

        // GET: Transactions/Edit
        public ActionResult Edit(int? id) {
            if(id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            Transaction transaction = db.TransactionData.Find(id);
            var bankAccount = db.BankAccountData.Single(b => b.Id == transaction.BankAccountId);
            if(transaction == null) {
                return HttpNotFound();
                }
            ViewBag.BankAccountId = new SelectList(db.BankAccountData, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData, "Id", "Name", transaction.CategoryId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
            }

        // POST: Transactions/Edit
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,CategoryId,BankAccountId,UserId,IsWithdrawl,IsDeleted,DateCreated,DateEdited,TransactionAmount,ReconsiliationAmount,TransactionDescription")] Transaction transaction) {
        public ActionResult Edit(Transaction transaction) {

            var user = db.Users.Find(User.Identity.GetUserId());
            var household = db.HouseHoldData.Where(u => u.Id == user.HouseHoldId);
            var bankAccount = db.BankAccountData.Single(b => b.Id == transaction.BankAccountId);

            if(ModelState.IsValid) {
            transaction.DateEdited = System.DateTimeOffset.Now;
            transaction.UserId = user.Id;

            db.TransactionData.Attach(transaction);
            db.Entry(transaction).Property("CategoryId").IsModified = true;
            db.Entry(transaction).Property("BankAccountId").IsModified = true;
            db.Entry(transaction).Property("TransactionAmount").IsModified = true;
            db.Entry(transaction).Property("ReconsiliationAmount").IsModified = true;
            db.Entry(transaction).Property("TransactionDescription").IsModified = true;

                if(db.Entry(transaction).Property(t => t.TransactionAmount).IsModified == true) {
                    if(transaction.TransactionAmount < 0 && transaction.IsWithdrawl == false) {
                        transaction.IsWithdrawl = true;
                        bankAccount.Balance = bankAccount.Balance + transaction.TransactionAmount;
                        transaction.DateEdited = System.DateTimeOffset.Now;
                    }
                    else {
                        transaction.IsWithdrawl = false;
                        bankAccount.Balance = bankAccount.Balance + transaction.TransactionAmount;
                        transaction.DateEdited = System.DateTimeOffset.Now;
                    }
                }

            db.SaveChanges();
            return RedirectToAction("Index", "Households");
            }
            
            ViewBag.BankAccountId = new SelectList(db.BankAccountData, "Id", "Name", transaction.BankAccountId);
            ViewBag.CategoryId = new SelectList(db.CategoryData, "Id", "Name", transaction.CategoryId);
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", transaction.UserId);
            return View(transaction);
            }

        // GET: Transactions/Delete
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

        // POST: Transactions/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id) {
            Transaction transaction = db.TransactionData.Find(id);
            transaction.IsDeleted = true;
            db.SaveChanges();
            return RedirectToAction("Index", "Households");
            }

        protected override void Dispose(bool disposing) {
            if(disposing) {
                db.Dispose();
                }
            base.Dispose(disposing);
            }
        }
    }
