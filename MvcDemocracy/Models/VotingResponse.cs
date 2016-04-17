using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
    
   public class VotingResponse
    {
        public int VotingId { get; set; }

        
        public string Description { get; set; }

        
        public int StateId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        
        [DataType(DataType.DateTime)]
        // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime DateTimeStart { get; set; }

        
        [DataType(DataType.DateTime)]
        // [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTimeEnd { get; set; }

        
        public bool IsForAllUsers { get; set; }

       
        public bool IsEnableBlankVote { get; set; }

        
        public int QuantityVotes { get; set; }

        
        public int QuantityBlankVotes { get; set; }

        
        public User Winner { get; set; }

       
        public  State States { get; set; }          
       
        public List<CandidateResponse> Candidates { get; set; }


    }
}
