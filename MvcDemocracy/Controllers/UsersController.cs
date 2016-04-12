using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcDemocracy.Models;
using System.IO;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using CrystalDecisions.CrystalReports.Engine;
//
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace MvcDemocracy.Controllers
{
    
    public class UsersController : Controller
    {             
        private MvcDemocracyContext db = new MvcDemocracyContext();

        [Authorize(Roles = "Admin")]
        public ActionResult DOC()
        {
            var report = this.GenerateUserRepost();

            var stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.WordForWindows);

            return File(stream, "application/doc","Users.doc");
        }

        [Authorize(Roles ="Admin")] 
        public ActionResult XLS()
        {
            var report = this.GenerateUserRepost();
            var stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.Excel);
            return File(stream, "application/xls", "Users.xls");
        }

         [Authorize(Roles = "Admin")]
         public ActionResult PDF()
        {
            var report = this.GenerateUserRepost();

            var stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            return File(stream, "application/pdf");
        }

        private ReportClass GenerateUserRepost()
        {
            var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var con = new SqlConnection(cs);

            var dt = new DataTable();

            var sql = "select * from Users order by lastName, FirstName";

            try
            {
                con.Open();

                var cmd = new SqlCommand(sql, con);
                var da = new SqlDataAdapter(cmd);
                da.Fill(dt);


            }
            catch (Exception ex)
            {

                ex.ToString();
            }

            var report = new ReportClass();
            report.FileName = Server.MapPath("/Reports/Users.rpt");
            //cargo el reporte en memoria:
            report.Load();
            report.SetDataSource(dt);

            return report;



        }

        [HttpPost]
        public ActionResult MySettings(UserSettingView view)
        {
            if (ModelState.IsValid)
            {
                //Upload Image:
                string path = string.Empty;
                string picture = string.Empty;

                if (view.NewPhoto != null)
                {
                    picture = Path.GetFileName(view.NewPhoto.FileName);
                    path = Path.Combine(Server.MapPath("~/Content/Photos"), picture);
                    view.NewPhoto.SaveAs(path);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        view.NewPhoto.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }

                var user = db.Users.Find(view.UserId);

                user.Address = view.Address;
                user.FirstName = view.FirstName;
                user.Grade = view.Grade;
                user.Group = view.Group;
                user.lastName = view.lastName;
                user.Phone = view.Phone;

                if (!string.IsNullOrEmpty(picture))
                {
                    user.Photo = string.Format("~/Content/Photos/{0}", picture);
                }


                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index","Home");
            }

            return View(view);
        }

        [Authorize(Roles = "User")]
        public ActionResult MySettings()
        {
            var user = db.Users.Where(u => u.UserName == this.User.Identity.Name).FirstOrDefault();

            var view = new UserSettingView
            {
                Address = user.Address,
                FirstName = user.FirstName,
                Grade = user.Grade,
                Group = user.Group,
                lastName = user.lastName,
                Phone = user.Phone,
                Photo = user.Photo,
                UserId = user.UserId,
                UserName = user.UserName,
            };

            return View(view);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult OnOffAdmin(int id)
        {
            var user = db.Users.Find(id);

            if (user != null)
            {
                var userContext = new ApplicationDbContext();

                var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

                var userASP = userManager.FindByEmail(user.UserName);

                if (userASP != null)
                {
                    if (userManager.IsInRole(userASP.Id, "Admin"))
                    {
                        userManager.RemoveFromRole(userASP.Id, "Admin");   
                    }
                    else
                    {
                        userManager.AddToRole(userASP.Id, "Admin");
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        // GET: Users
        public ActionResult Index()
        {
            var userContext = new ApplicationDbContext();

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(userContext));

            var users = db.Users.ToList();

            var userView = new List<UserIndexView>();

            foreach (var user in users)
            {
                var userASP = userManager.FindByEmail(user.UserName);

                userView.Add(new UserIndexView
                {

                    Address = user.Address,
                    Candidates = user.Candidates,
                    FirstName = user.FirstName,
                    Grade = user.Grade,
                    Group = user.Group,
                    GroupMembers = user.GroupMembers,
                    IsAdmin = userASP != null && userManager.IsInRole(userASP.Id, "Admin"),
                    lastName = user.lastName,
                    Phone = user.Phone,
                    Photo = user.Photo,
                    UserId = user.UserId,
                    UserName = user.UserName,

                });
            }

            return View(userView);
        }

        [Authorize(Roles = "Admin")]
        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }


        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserView userView)
        {
            if (!ModelState.IsValid)
            {
                return View(userView);
            }

            //Upload Image:
            string path = string.Empty;
            string picture = string.Empty;

            if (userView.Photo != null)
            {
                picture = Path.GetFileName(userView.Photo.FileName);
                path = Path.Combine(Server.MapPath("~/Content/Photos"), picture);
                userView.Photo.SaveAs(path);

                using (MemoryStream ms = new MemoryStream())
                {
                    userView.Photo.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
            }

            //Save record:
            var user = new User
            {
                Address = userView.Address,
                FirstName = userView.FirstName,
                Grade = userView.Grade,
                Group = userView.Group,
                lastName = userView.lastName,
                Phone = userView.Phone,
                Photo = picture == string.Empty ? string.Empty : string.Format("~/Content/Photos/{0}", picture),
                UserName = userView.UserName
            };

            db.Users.Add(user);

            try
            {
                db.SaveChanges();

                this.CreateASPuser(userView);

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null
                    && ex.InnerException.InnerException.Message.Contains("userNameIndex"))
                {
                    ViewBag.Error = "The Email has already used for another User.";
                }
                else
                {
                    ViewBag.Error = ex.Message;
                }

                return View(userView);
            }

            return RedirectToAction("Index");
        }

        private void CreateASPuser(UserView userView)
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

        [Authorize(Roles = "Admin")]
        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.Find(id);

            if (user == null)
            {
                return HttpNotFound();
            }

            var userView = new UserView
            {
                Address = user.Address,
                FirstName = user.FirstName,
                Grade = user.Grade,
                Group = user.Group,
                lastName = user.lastName,
                Phone = user.Phone,
                UserId = user.UserId,
                UserName = user.UserName,
            };

            return View(userView);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( UserView userView)
        {
            if (ModelState.IsValid)
            {
                //Upload image
                string path = string.Empty;
                string picture = string.Empty;

                if (userView.Photo != null)
                {
                    picture = Path.GetFileName(userView.Photo.FileName);
                    //~ genera una ruta relativa:
                    path = Path.Combine(Server.MapPath("~/Content/Photos"), picture);
                    userView.Photo.SaveAs(path);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        userView.Photo.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }

                var user = db.Users.Find(userView.UserId);

                user.Address = userView.Address;
                user.FirstName = userView.FirstName;
                user.Grade = userView.Grade;
                user.Group = userView.Group;
                user.lastName = userView.lastName;
                user.Phone = userView.Phone;

                if (!string.IsNullOrEmpty(picture))
                {
                    user.Photo = string.Format("~/Content/Photos/{0}", picture);
                }
         

                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }


            return View(userView);
        }

        [Authorize(Roles = "Admin")]
        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);

            db.Users.Remove(user);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null && ex.InnerException.InnerException != null && 
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    ViewBag.Error = "Can't delete the record, because has related records.....";
                }
                else
                {
                    ViewBag.Error = ex.Message;
                }

                return View(user);
            }


            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
