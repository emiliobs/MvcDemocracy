using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
   public class VotingResponse : Voting
    {
             
        public  State MyStates { get; set; }
                       
               
        public  List<VotingDetail> MyVotingDetails { get; set; }

        public User Winner;
    }
}
