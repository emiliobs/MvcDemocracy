﻿using System;
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
        public string Description { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name ="State")]
        public int StateId { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name = "Date time start")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode =true)]
        public DateTime DateTimeStart { get; set; }

        [Display(Name = "Date time end")]
        [Required(ErrorMessage = "The field {0} is Requered")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString ="{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateTimeEnd { get; set; }

        [Display(Name = "Is For All Users?")]
        [Required(ErrorMessage = "The field {0} is Requered")]
        public int IsForAllUsers { get; set; }

        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name = "Is Enable Blank Vote?")]
        public int IsEnableBlankVote { get; set; }

        [Display(Name = "Quantity Blank Votes")]
        public int QuantityBlankVotes { get; set; }

        [Display(Name = "Winner")]
        public int CandidateWinId { get; set; }

    }
}
