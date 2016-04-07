using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcDemocracy.Migrations;
using MvcDemocracy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MvcDemocracy
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            //me mira si la base de datos cambio, en ls migraciones automaticas:
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MvcDemocracyContext, Configuration>());

            //método que garantiza que simepre hay un super usuario en el sistema(admin)
            this.CheckSuperUser();   

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void CheckSuperUser()
        {
            var userContext = new ApplicationDbContext();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var db = new MvcDemocracyContext();

            //check si existen los roles user and Admin:
            this.CheckRole("Admin", userContext);
            this.CheckRole("User", userContext);

            //valido si el usuario existe:
            var user = db.Users.Where(u=>u.UserName.ToLower().Equals("barrera_emilio@hotmail.com")).FirstOrDefault();

            if (user == null)
            {
                //Save record:
                user = new User
                {
                    Address = "Calle madrid 55",
                    FirstName = "Emilio",
                    lastName = "Barrera",
                    Phone = "693661995",
                    UserName = "barrera_emilio@hotmail.com",
                    Photo = "~/Content/Photos/fondo.jpg",
                };

                db.Users.Add(user);
                db.SaveChanges();
            }

             //el la tabla asp de la BD:
            var userASP = userManager.FindByName(user.UserName);

            if (userASP == null)
            {
                userASP = new ApplicationUser
                {
                  UserName = user.UserName,
                  Email = user.UserName,
                  PhoneNumber = user.Phone,
                };

                userManager.Create(userASP, "Eabs+++++55555");
            }

            userManager.AddToRole(userASP.Id, "Admin");
        }

        private void CheckRole(string roleName, ApplicationDbContext  userContext)
        {
            //User management:             
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(userContext));

            //Check to see if role Exists, if not create it:
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));

            }
        }

        
    }
}
