using MvcDemocracy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace MvcDemocracy.Controllers
{
    public class StatesController : Controller
    {
        MvcDemocracyContext db = new MvcDemocracyContext();
        /// <summary>
        /// List all states
        /// </summary>
        /// <returns>the view</returns>
        // GET: States
        [HttpGet]
        public ActionResult Index()
        {
            return View(db.States.ToList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create a new state
        /// </summary>
        /// <param name="state">the state</param>
        /// <returns>the view</returns>

        [HttpPost]
        public ActionResult Create(State state)
        {
            if (ModelState.IsValid)
            {
                db.States.Add(state);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(state);
        }

        /// <summary>
        /// Edit all state
        /// </summary>
        /// <returns>the view</returns>
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var state = db.States.Find(id);

            if (state == null)
            {
                return HttpNotFound();
            }

            return View(state);
        }

        [HttpPost]
        public ActionResult Edit(State state)
        {
            if (ModelState.IsValid)
            {
                db.Entry(state).State = EntityState.Modified;

                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(state);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var state = db.States.Find(id);

            if (state == null)
            {
                return HttpNotFound();
            }

            return View(state);
        }

        [HttpGet]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var state = db.States.Find(id);

            if (state == null)
            {
                return HttpNotFound();
            }

            return View(state);
        }

        [HttpPost]
        public ActionResult Delete(int id, State state)
        {
             state = db.States.Find(id);

          

            if (state == null)
            {
                return HttpNotFound();
            }

            db.States.Remove(state);
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