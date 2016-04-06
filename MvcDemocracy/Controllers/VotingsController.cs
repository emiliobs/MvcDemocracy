using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcDemocracy.Models;

namespace MvcDemocracy.Controllers
{
    [Authorize]
    public class VotingsController : Controller
    {
        private MvcDemocracyContext db = new MvcDemocracyContext();

        public ActionResult DeleteGroup(int id)
        {

            //buscar el id a eliminar:
            var votingGroup = db.VotingGroups.Find(id);

            if (votingGroup != null)
            {
                db.VotingGroups.Remove(votingGroup);
                db.SaveChanges();
            }



            return RedirectToAction($"Details/{votingGroup.VotingId}");
        }

        public ActionResult DeleteCandidate(int id)
        {
            //buscar el id a eliminar:
            var cadidate = db.Candidates.Find(id);

            if (cadidate != null)
            {
                db.Candidates.Remove(cadidate);
                db.SaveChanges();
            }



            return RedirectToAction($"Details/{cadidate.VotingId}");
        }

        [HttpGet]
        public ActionResult AddCandidate(int id)
        {
            var view = new AddCandidateView
            {
                VotingId = id,
            };

            ViewBag.UserId = new SelectList(db.Users.OrderBy(u => u.FirstName).ThenBy(u => u.lastName), "UserId", "FullName").ToList();



            return View(view);
        }

        [HttpPost]
        public ActionResult AddCandidate(AddCandidateView view)
        {


            if (ModelState.IsValid)
            {
                //busco si grupo existe o si ya esta seleccionado:
                var candidate = db.Candidates.Where(c => c.VotingId == view.VotingId &&
                                 c.UserId == view.UserId).FirstOrDefault();//ejecuto la funcion link:

                //Aquí a ver si si lo encontro:
                if (candidate != null)
                {
                    ModelState.AddModelError("", "The Candidate already belong to Voting.");

                    //ViewBag.Error = "The group already belong to Voting.";

                    ViewBag.UserId = new SelectList(db.Users.OrderBy(u => u.FirstName).ThenBy(u => u.lastName), "UserId", "FullName").ToList();

                    return View(view);
                }

                //Si no existe debo crear el objeto de bd: y despues de crear el objeto lo envio a la base de datos:
                candidate = new  Candidate
                {
                    UserId = view.UserId,
                    VotingId = view.VotingId,
                };

                db.Candidates.Add(candidate);
                db.SaveChanges();

                return RedirectToAction($"Details/{view.VotingId}");
            }

            ViewBag.UserId = new SelectList(db.Users.OrderBy(u => u.FirstName).ThenBy(u => u.lastName), "UserId", "FullName").ToList();


            return View(view);

        }

        [HttpPost]
        public ActionResult AddGroup(VotingGroup  addGroupView)
        {
            if (ModelState.IsValid)
            {
                //busco si grupo existe o si ya esta seleccionado:
                var votingGroup = db.VotingGroups.Where(vg => vg.VotingId == addGroupView.VotingId && 
                                 vg.GroupId == addGroupView.GroupId).FirstOrDefault();//ejecuto la funcion link:

                //Aquí a ver si si lo encontro:
                if (votingGroup != null)
                {
                    ModelState.AddModelError("", "The group already belong to Voting.");

                    //ViewBag.Error = "The group already belong to Voting.";

                    ViewBag.GroupId = new SelectList(db.Groups.OrderBy(g => g.Description), "GroupId", "Description");

                    return View(addGroupView);
                }

                //Si no existe debo crear el objeto de bd: y despues de crear el objeto lo envio a la base de datos:
                votingGroup = new VotingGroup
                {
                  GroupId = addGroupView.GroupId,
                  VotingId = addGroupView.VotingId,
                };

                db.VotingGroups.Add(votingGroup);
                db.SaveChanges();

                return RedirectToAction($"Details/{addGroupView.VotingId}");
            }

            ViewBag.GroupId = new SelectList(db.Groups.OrderBy(g => g.Description), "GroupId", "Description");

            return View(addGroupView);
        }

        [HttpGet]
        public ActionResult AddGroup(int id)
        {
            ViewBag.GroupId = new SelectList(db.Groups.OrderBy(g => g.Description), "GroupId", "Description");

            var view = new VotingGroup
            {
                VotingId = id,
            };

            return View(view);
        }

        // GET: Votings
        public ActionResult Index()
        {
            var votings = db.Votings.Include(v => v.States);
            return View(votings.ToList());
        }

        // GET: Votings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voting voting = db.Votings.Find(id);
            if (voting == null)
            {
                return HttpNotFound();
            }

            var view = new DetailsVotingView
            {
              Candidates = voting.Candidates.ToList(),
              CandidateWinId = voting.CandidateWinId,
              DateTimeEnd = voting.DateTimeEnd,
              DateTimeStart = voting.DateTimeStart,
              Description = voting.Description,
              IsEnableBlankVote = voting.IsEnableBlankVote,
              IsForAllUsers = voting.IsForAllUsers,
              QuantityBlankVotes = voting.QuantityBlankVotes,
              QuantityVotes = voting.QuantityVotes,
              Remarks = voting.Remarks,
              StateId = voting.StateId,
              VotingGroups = voting.VotingGroups.ToList(),
              VotingId = voting.VotingId,
              States = voting.States,

            };

            return View(view);
        }

        // GET: Votings/Create
        public ActionResult Create()
        {
            ViewBag.StateId = new SelectList(db.States, "StateId", "Description");

            var view = new VotingView
            {
                DateStart = DateTime.Now,
                DateEnd = DateTime.Now,
            };

            return View(view);
        }

        // POST: Votings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VotingView votingView)
        {
            if (ModelState.IsValid)
            {
                var voting = new Voting
                {
                  DateTimeStart = votingView.DateStart.AddHours(votingView.TimeStart.Hour).AddMinutes(votingView.TimeStart.Minute),
                  DateTimeEnd   = votingView.DateEnd.AddHours(votingView.TimeEnd.Hour).AddMinutes(votingView.TimeEnd.Minute),
                  Description = votingView.Description,
                  IsEnableBlankVote = votingView.IsEnableBlankVote,
                  IsForAllUsers = votingView.IsForAllUsers,
                  Remarks = votingView.Remarks,
                  StateId = votingView.StateId,

                };

                db.Votings.Add(voting);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StateId = new SelectList(db.States, "StateId", "Description", votingView.StateId);
            return View(votingView);
        }

        // GET: Votings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Voting voting = db.Votings.Find(id);

            if (voting == null)
            {
                return HttpNotFound();
            }

            var view = new VotingView
            {
               DateEnd = voting.DateTimeEnd,
               DateStart = voting.DateTimeStart,
               Description = voting.Description,
               IsEnableBlankVote = voting.IsEnableBlankVote,
               IsForAllUsers = voting.IsForAllUsers,
               Remarks = voting.Remarks,
               StateId = voting.StateId,
               TimeEnd = voting.DateTimeEnd,
               TimeStart = voting.DateTimeStart,
               VotingId = voting.VotingId,
            };

            ViewBag.StateId = new SelectList(db.States, "StateId", "Description", voting.StateId);

            

            return View(view);
        }

        // POST: Votings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VotingView votingView)
        {
            if (ModelState.IsValid)
            {

                var voting = new Voting
                {
                    DateTimeStart = votingView.DateStart.AddHours(votingView.TimeStart.Hour).AddMinutes(votingView.TimeStart.Minute),
                    DateTimeEnd = votingView.DateEnd.AddHours(votingView.TimeEnd.Hour).AddMinutes(votingView.TimeEnd.Minute),
                    Description = votingView.Description,
                    IsEnableBlankVote = votingView.IsEnableBlankVote,
                    IsForAllUsers = votingView.IsForAllUsers,
                    Remarks = votingView.Remarks,
                    StateId = votingView.StateId,
                    VotingId = votingView.VotingId,

                };

                db.Entry(voting).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StateId = new SelectList(db.States, "StateId", "Description", votingView.StateId);

            return View(votingView);
        }

        // GET: Votings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voting voting = db.Votings.Find(id);
            if (voting == null)
            {
                return HttpNotFound();
            }
            return View(voting);
        }

        // POST: Votings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Voting voting = db.Votings.Find(id);

            db.Votings.Remove(voting);

            try
            {
                db.SaveChanges();
            }
            catch (Exception ex)
            {

                if (ex.InnerException != null && ex.InnerException.InnerException != null && 
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    ViewBag.Error = "Can't delete the record, because has related records......";
                }
                else
                {
                    ViewBag.Error = ex.Message;
                }

                return View(voting);
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
