using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MvcDemocracy.Models
{
    public class Candidate
    {
         [Key]
        public int CandidateId { get; set; }
        public int VotingId { get; set; }
        public int UserId { get; set; }
        public int QuantityVotes { get; set; }


        //Relación varios:
        [JsonIgnore]
        public virtual Voting Voting { get; set; }
        [JsonIgnore]
        public virtual User User  { get; set; }

        //relacion uno a muchos:
        [JsonIgnore]
        public virtual ICollection<VotingDetail> VotingDetails { get; set; }


    }
}