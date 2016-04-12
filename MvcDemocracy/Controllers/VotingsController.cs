using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MvcDemocracy.Models;
using CrystalDecisions.CrystalReports.Engine;
using System.Configuration;
using System.Data.SqlClient;

namespace MvcDemocracy.Controllers
{

    public class VotingsController : Controller
    {
        private MvcDemocracyContext db = new MvcDemocracyContext();

        [Authorize(Roles = "User")]
        public ActionResult Results()
        {
            var votings = db.Votings.Include(v => v.States);
            return View(votings.ToList());
        }

        [Authorize(Roles = "User")]
        public ActionResult ShowResults(int id)
        {
            var report = this.GenerateResultRepost(id);

            var stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            return File(stream, "application/pdf");
        }

        private ReportClass GenerateResultRepost(int id)
        {
            var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var con = new SqlConnection(cs);

            var dt = new DataTable();

            var sql = @"SELECT  Votings.VotingId, Votings.Description AS Voting, States.Description AS State, 
                       Users.FirstName + ' ' + Users.lastName AS Candidate, Candidates.QuantityVotes
                        FROM   Candidates INNER JOIN
                        Users ON Candidates.UserId = Users.UserId INNER JOIN
                        Votings ON Candidates.VotingId = Votings.VotingId INNER JOIN
                        States ON Votings.StateId = States.StateId
                        where Votings.VotingId =" + id;

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
            report.FileName = Server.MapPath("/Reports/Results.rpt");
            //cargo el reporte en memoria:
            report.Load();
            report.SetDataSource(dt);

            return report;
        }

        [Authorize(Roles ="User")]
        public ActionResult VoteForCandidate(int candidateId, int votingId)
        {
            //BUscamos el usuario:
            var user = db.Users.Where(u => u.UserName == this.User.Identity.Name).FirstOrDefault();//Usuario logiado:

            //valido el usuario:
            if (user == null)//ese man no estas:
            {
                return RedirectToAction("Index","Home");// lo mando al index del home por quue usted no puede votat://
            }

            //valido el candidato:
            var candidate = db.Candidates.Find(candidateId);

            if (candidate == null)
            {
                return RedirectToAction("Index", "Home");

            }

            var voting = db.Votings.Find(votingId);

            if (voting == null)
            {
                return RedirectToAction("Index", "Home");
            }

            //método  VoteCandidate(user,candidate,voting)
            if (this.VoteCandidate(user, candidate,voting))
            {
                return RedirectToAction("MyVotings");
            }


            return RedirectToAction("Index", "Home");
        }

        private bool VoteCandidate(User user, Candidate candidate, Voting voting)
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var votingDetail = new VotingDetail
                {
                  CandidateId = candidate.CandidateId,
                  DateTime = DateTime.Now,
                  UserId = user.UserId,
                  VotingId = voting.VotingId,
                };

                db.VotingDetails.Add(votingDetail);

                candidate.QuantityVotes++;
                db.Entry(candidate).State = EntityState.Modified;

                voting.QuantityVotes++;
                db.Entry(voting).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {

                    transaction.Rollback();
                    
                }         
            }

            return false;
        }

        [Authorize(Roles = "User")]
        public ActionResult Vote(int votingId)
        {
            //BUsco el votingid enviado desde la vista: 
            var voting = db.Votings.Find(votingId);//objeto voting:

            var view = new VotingVoteView
            {
               DateTimeEnd = voting.DateTimeEnd,
               DateTimeStart = voting.DateTimeStart,
               Description = voting.Description,
               IsEnableBlankVote = voting.IsEnableBlankVote,
               IsForAllUsers = voting.IsForAllUsers,
               MyCandidates = voting.Candidates.ToList(),
               Remarks = voting.Remarks,
               VotingId = voting.VotingId,
            };



            return View(view);
        }

        [Authorize(Roles ="User")]
        public ActionResult MyVotings()
        {
            var user = db.Users.Where(u => u.UserName == this.User.Identity.Name).FirstOrDefault();

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "There an error with the current user. call the support.");

                return View();
            }

            //Get event voting for the current time:
            var state = this.GetState("Open");

            var votings = db.Votings.Where(v => v.StateId == state.StateId && 
                          v.DateTimeStart <= DateTime.Now && 
                          v.DateTimeEnd >=DateTime.Now).
                          Include(v => v.Candidates).
                          Include(v => v.VotingGroups).
                          Include(v => v.States).ToList();

            //Discart evets in the wich the user already vote:
            foreach (var voting in votings.ToList())//cuando a una lista le ponemos tolist(), es una copia en memoria de la lista:
            {
                //int userId = user.UserId;
                //int votingId = voting.VotingId;

                var votingDetail = db.VotingDetails.Where(vd => vd.VotingId == voting.VotingId && vd.UserId == user.UserId).FirstOrDefault();

                if (votingDetail != null)
                {
                    votings.Remove(voting);

                }
            }

            //for (int i  = 0; i  < votings.Count; i ++)
            //{
            //    int userId = user.UserId;
            //    int votingId = votings[i].VotingId;

            //    var votingDetail = db.VotingDetails.Where(vd => vd.VotingId == votingId && vd.UserId == userId).FirstOrDefault();

            //    if (votingDetail != null)
            //    {
            //        votings.RemoveAt(i);

            //    }
            //}

            //Discart events by groups in wich the user are not included:
            foreach (var voting in votings.ToList())
            {
                if (!voting.IsForAllUsers)
                {
                    bool userBelongsToGroup = false;

                    foreach (var votinGroup in voting.VotingGroups)
                    {
                        var userGroup = votinGroup.Group.GroupMembers.Where(gm => gm.UserId == user.UserId).FirstOrDefault();

                        if (userGroup != null)
                        {
                            userBelongsToGroup = true;
                            break;
                        }
                    }

                    if (!userBelongsToGroup)
                    {
                        votings.Remove(voting);
                    }
                }
            }

            //for (int i = 0; i < votings.Count; i++)
            //{
            //    if (!votings[i].IsForAllUsers)//si no es para todos los usuario haga esto:
            //    {
            //        bool userBelongsToGroup = false;

            //        foreach (var votingGroup in votings[i].VotingGroups)
            //        {
            //            //pregunto si estoy en el grupo:
            //            var userGroup = votingGroup.Group.GroupMembers.Where(gm => gm.UserId == user.UserId).FirstOrDefault();

            //            //pregunso si si lo encontro o no(al usuario en algun grupo):
            //            if (userGroup != null)
            //            {
            //                userBelongsToGroup = true;
            //                break;
            //            }     
            //        }

            //        //si es null es por que no pertence al grupo que dael derecho de votación:
            //        if (!userBelongsToGroup)//si el usuario no pertenece al grupo
            //        {
            //            votings.RemoveAt(i);
            //        } 
            //    }

                    
            //}

            return View(votings);
        }

        private State GetState(string stateName)
        {

            //Aseguro que siempre el estado sea open si no esxiste:
            var state = db.States.Where(s => s.Description == stateName).FirstOrDefault();

            if (state == null)
            {
                state = new State
                {
                    Description = stateName,
                };

                db.States.Add(state);
                db.SaveChanges();

            }

            return state;
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        // GET: Votings
        public ActionResult Index()
        {
            var votings = db.Votings.Include(v => v.States);
            return View(votings.ToList());
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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
