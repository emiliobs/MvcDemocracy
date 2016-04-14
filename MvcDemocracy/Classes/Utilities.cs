using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MvcDemocracy.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace MvcDemocracy.Classes
{
   public class Utilities
    {

        public static string UploadPhoto(HttpPostedFileBase file)
        {
            //Upload Image:
            string path = string.Empty;
            string picture = string.Empty;

            if (file != null)
            {
                picture = Path.GetFileName(file.FileName);
                path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Photos"), picture);
                file.SaveAs(path);

                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
            }

            return picture;
        }

        public static void CreateASPuser(UserView userView)
        {
            //User management:
            var userContext = new ApplicationDbContext();
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(userContext));

            //Creaate User Role:
            string roleName = "User";

            //Check to see if role Exists, if not create it:
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new IdentityRole(roleName));

            }

            //Creaate the ASP NET User:
            var userASP = new ApplicationUser
            {
                UserName = userView.UserName,
                Email = userView.UserName,
                PhoneNumber = userView.Phone
            };

            userManager.Create(userASP, userASP.UserName); //la contraseña es el mismo correo:

            //Add user to role:
            userASP = userManager.FindByName(userView.UserName);
            userManager.AddToRole(userASP.Id, "User");
        }


    }
}
