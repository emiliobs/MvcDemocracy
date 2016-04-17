using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcDemocracy.Models
{
    public class CandidateResponse
    {
        public int CandidateId { get; set; }
       
        public int QuantityVotes { get; set; }
     
       
        public  User User { get; set; }

    }
}