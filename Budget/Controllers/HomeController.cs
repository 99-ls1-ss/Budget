﻿using Budget.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Budget.Controllers {
    public class HomeController : Controller {
        private ApplicationDbContext db = new ApplicationDbContext();


        public ActionResult Index(int? id) {
            //HouseHold houseHold = db.HouseHoldData.Find(id);
            //ApplicationUser member = db.Users.Find(id);

            //if(User.Identity.IsAuthenticated == true) {                
            //    return RedirectToAction("Index", "HouseHolds", new { id = houseHold.Id });                
            //}
            //else {
                return View();
            //}
            
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}