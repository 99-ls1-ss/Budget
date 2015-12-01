using Budget.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.Owin;

namespace Budget.Helpers {
    public static class extensions {
        public static void SendNotification(this ApplicationUser user, string subject, string body) {
            EmailService es = new EmailService();
            IdentityMessage im = new IdentityMessage {
                Destination = user.Email,
                Subject = subject,
                Body = body
            };
            es.SendAsync(im);
        }

        public static string GetHouseholdid(this IIdentity user) {
            var claimsIdentity = (ClaimsIdentity)user;
            var HousholdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "HouseholdId");

            if(HousholdClaim != null) {
                return HousholdClaim.Value;
            }
            else {
                return null;
            }
        }

        public static bool IsInHousehold(this IIdentity user) {
            var cUser = (ClaimsIdentity)user;
            var hid = cUser.Claims.FirstOrDefault(c => c.Type == "HouseholdId");
            return (hid != null && !string.IsNullOrWhiteSpace(hid.Value));
        }

        public static async Task RefreshAuthentication(this HttpContextBase context, ApplicationUser user) {
            context.GetOwinContext().Authentication.SignOut();
            await context.GetOwinContext().Get<ApplicationSignInManager>().SignInAsync(user, isPersistent: false, rememberBrowser: false);
        }
    }
}