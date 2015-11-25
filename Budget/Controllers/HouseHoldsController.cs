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

namespace Budget.Controllers {
    public class HouseHoldsController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: HouseHolds
        public ActionResult Index() {
            return View(db.HouseHoldData.ToList());
            }

        // GET: HouseHolds/Details/5
        public ActionResult Details(int? id) {
            if(id == null) {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            var householdId = db.HouseHoldData.Find(id);
            var selected = householdId.Users.Select(u => u.Id);

            HouseholdUser householdUser = new HouseholdUser() {
                Members = new MultiSelectList(db.Users, "Id", "DisplayName", selected),
                HouseHolds = householdId
            };

            //HouseHold houseHold = db.HouseHoldData.Find(id);
            //if(houseHold == null) {
            //    return HttpNotFound();
            //    }
            return View(householdUser);
            }


        //var projectId = db.ProjectsData.Find(id);
        //var selected = projectId.Users.Select(u => u.Id);

        //ProjectUsersModel projectUsersModel = new ProjectUsersModel() {
        //    Users = new MultiSelectList(db.Users, "Id", "DisplayName", selected),
        //    Project = projectId
        //};
        //return View(projectUsersModel);

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
