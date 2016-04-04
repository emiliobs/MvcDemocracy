using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
    public class VotingView
    {
        public int VotingId { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [StringLength(50, ErrorMessage = ("The field {0} can contain maximun {1} an minimum {2} character"), MinimumLength = 3)]
        [Display(Name = "Voting Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name = "State")]
        public int StateId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name = "Date Start")]
        [DataType(DataType.Date)]
        // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime DateStart { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name = "Time Start")]
        [DataType(DataType.Time)]
        // [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime TimeStart { get; set; }

        [Display(Name = "Date End")]
        [Required(ErrorMessage = "The field {0} is Requered")]
        [DataType(DataType.Date)]
        // [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateEnd { get; set; }

        [Display(Name = "Time End")]
        [Required(ErrorMessage = "The field {0} is Requered")]
        [DataType(DataType.Time)]
        // [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TimeEnd { get; set; }

        [Display(Name = "Is For All Users?")]
        [Required(ErrorMessage = "The field {0} is Requered")]
        public bool IsForAllUsers { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name = "Is Enable Blank Vote?")]
        public bool IsEnableBlankVote { get; set; }
    }
}
