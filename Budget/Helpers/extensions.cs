using Budget.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}