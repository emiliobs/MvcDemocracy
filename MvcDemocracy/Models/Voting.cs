using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
   public class Voting
    {
        [Key]
        public int VotingId { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [StringLength(50, ErrorMessage = ("The field {0} can contain maximun {1} an minimum {2} character"), MinimumLength = 3)]
        [Display(Name = "Voting Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name ="State")]
        public int StateId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name = "Date time start")]
        [DataType(DataType.DateTime)]
       // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime DateTimeStart { get; set; }

        [Display(Name = "Date time end")]
        [Required(ErrorMessage = "The field {0} is Requered")]
        [DataType(DataType.DateTime)]
       // [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTimeEnd { get; set; }

        [Display(Name = "Is For All Users?")]
        [Required(ErrorMessage = "The field {0} is Requered")]
        public bool IsForAllUsers { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name = "Is Enable Blank Vote?")]
        public bool IsEnableBlankVote { get; set; }

        [Display(Name = "Quantity Votes")]
        public int QuantityVotes { get; set; }

        [Display(Name = "Quantity Blank Votes")]
        public int QuantityBlankVotes { get; set; }

        [Display(Name = "Winner")]
        public int CandidateWinId { get; set; }

        [Display(Name = "State")]
        public virtual State States { get; set; }


        //realción unica:
        public virtual ICollection<VotingGroup> VotingGroups { get; set; }
        public virtual ICollection<Candidate> Candidates { get; set; }

        public virtual ICollection<VotingDetail> VotingDetails { get; set; }




    }
}
