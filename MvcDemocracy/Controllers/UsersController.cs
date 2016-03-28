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

namespace MvcDemocracy.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private MvcDemocracyContext db = new MvcDemocracyContext();

        // GET: Users
        public ActionResult Index()
        {
            return View(db.Users.ToList());
        }

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
            if (ModelState.IsValid)
            {

                //Upload image
                string path = string.Empty;
                string picture = string.Empty;

                if (userView.Photo !=null)
                {
                    picture = Path.GetFileName(userView.Photo.FileName);
                    //~ genera una ruta relativa:
                    path = Path.Combine(Server.MapPath("~/Content/Photos"), picture);
                    userView.Photo.SaveAs(path);

                    using (MemoryStream ms = new MemoryStream ())
                    {
                        userView.Photo.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                }

                //genero un objeto user atravez del objeto userview:
                var user = new User
                {
                    Address = userView.Address,
                    FirstName = userView.FirstName,
                    Grade = userView.Grade,
                    Group = userView.Group,
                    lastName = userView.lastName,
                    Phone = userView.Phone,
                    Photo = picture == string.Empty ? string.Empty : string.Format("~/Content/Photos/{0}", picture),
                    UserName = userView.UserName,
                };

                //save record:
                db.Users.Add(user);

                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                    if (ex.InnerException != null && ex.InnerException.InnerException != null &&       
                        ex.InnerException.InnerException.Message.Contains("UserNameIndex"))
                    {
                        ViewBag.Error = "The Email has already used for another user.";
                    }
                    else
                    {
                        ViewBag.Error = ex.Message;
                    }

                    return View(userView);

                }

                return RedirectToAction("Index");
            }

            return View(userView);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
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

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,UserName,FirstName,lastName,Phone,Address,Grade,Group,Photo")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(user);
        }

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
            db.SaveChanges();
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
