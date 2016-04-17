using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using MvcDemocracy.Models;
using MvcDemocracy.Classes;

namespace MvcDemocracy.Controllers.API
{
    [RoutePrefix("api/Votings")]
   public class VotingController : ApiController
    {
        private MvcDemocracyContext db = new MvcDemocracyContext();

        [HttpGet]
        [Route("{userId}")]
        public IHttpActionResult MyVotings(int userID)
        {
            var user = db.Users.Find(userID);

            if (user == null)
            {
                return this.BadRequest("User no Found.");
            }

            var votings = Utilities.MyVotings(user);

            var votingsResponse = new List<VotingResponse>();

            foreach (var voting in votings)
            {

                User winner = null;

                if (voting.CandidateWinId !=0)
                {
                    winner = db.Users.Find(voting.CandidateWinId);
                }

                var candidates = new List<CandidateResponse>();
                foreach (var candidate in voting.Candidates)
                {
                    candidates.Add(new CandidateResponse
                    {
                        CandidateId = candidate.CandidateId,
                        QuantityVotes = candidate.QuantityVotes,
                        User =candidate.User,

                    });
                }

                votingsResponse.Add(new VotingResponse
                {
                    DateTimeEnd = voting.DateTimeEnd,
                    DateTimeStart = voting.DateTimeStart,
                    Description = voting.Description,
                    IsEnableBlankVote = voting.IsEnableBlankVote,
                    IsForAllUsers = voting.IsForAllUsers,
                    Candidates = candidates,                  
                    VotingId = voting.VotingId,
                    Winner = winner,

                 
                });
            }

            return this.Ok(votingsResponse);
        }

        //cerrar las conexions a DB:
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
