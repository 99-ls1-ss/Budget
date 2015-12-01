using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web;
using Budget.Helpers;
using System.Web.Routing;
using Microsoft.AspNet.Identity.Owin;

namespace Budget.Models {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser {

        public ApplicationUser() {
            this.BankAccounts = new HashSet<BankAccount>();
            this.Transactions = new HashSet<Transaction>();
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public int? HouseHoldId { get; set; }

        public virtual ICollection<BankAccount> BankAccounts { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
        public virtual HouseHold HouseHold { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("HouseholdId", HouseHoldId.ToString()));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) {
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }
        
        public System.Data.Entity.DbSet<Models.BankAccount> BankAccountData { get; set; }
        public System.Data.Entity.DbSet<Models.BudgetItems> BudgetData { get; set; }
        public System.Data.Entity.DbSet<Models.Category> CategoryData { get; set; }
        public System.Data.Entity.DbSet<Models.HouseHold> HouseHoldData { get; set; }
        public System.Data.Entity.DbSet<Models.Member> MemberData { get; set; }
        public System.Data.Entity.DbSet<Models.Transaction> TransactionData { get; set; }        

    }

    public class AuthorizeHouseholdRequired : AuthorizeAttribute {
        protected override bool AuthorizeCore(HttpContextBase httpContext) {
            var isAuthorized = base.AuthorizeCore(httpContext);
            if(!isAuthorized) {
                return false;
            }
            return httpContext.User.Identity.IsInHousehold();
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext) {
            if(!filterContext.HttpContext.User.Identity.IsAuthenticated) {
                base.HandleUnauthorizedRequest(filterContext);
            }
            else {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Home", action = "CreateJoinHousehold" }));
            }
        }
    }
}