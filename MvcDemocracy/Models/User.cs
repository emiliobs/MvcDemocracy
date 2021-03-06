﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
   public class User
    {
        [Key]
        public int UserId { get; set; }

        [Index("UserNameIndex", IsUnique =true)]
        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(100, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characteres", MinimumLength = 7)]
        [Display(Name ="E-Mail")]
        public string UserName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characteres", MinimumLength = 2)]
        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Text)]
        [StringLength(50, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characteres", MinimumLength = 2)]
        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name ="Last Name")]
        public string lastName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(20, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characteres", MinimumLength = 7)]
        [Required(ErrorMessage = "The field {0} is required")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name ="User")]
        public string FullName { get { return string.Format($"{this.FirstName} {this.lastName}"); } }


        [StringLength(100, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characteres", MinimumLength = 7)]
        [Required(ErrorMessage = "The field {0} is required")]
        public string Address { get; set; }

        public string Grade { get; set; }

        public string Group { get; set; }

        [DataType(DataType.ImageUrl)]
        [StringLength(200, ErrorMessage = "The field {0} can contain maximun {1} and minimum {2} characteres", MinimumLength = 5)]
        public string Photo { get; set; }

         [JsonIgnore]
        public virtual ICollection<GroupMember> GroupMembers { get; set; }

        [JsonIgnore]
        public virtual ICollection<Candidate> Candidates { get; set; }

        [JsonIgnore]
        public virtual ICollection<VotingDetail> VotingDetails { get; set; }


    }
}
