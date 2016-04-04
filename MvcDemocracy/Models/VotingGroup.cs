using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
    public class VotingGroup
    {
        [Key]
        public int VotingGroupId { get; set; }

        [Required(ErrorMessage = "You must select a Group.")]
        public int GroupId { get; set; }

        [Required(ErrorMessage = "You must select a Voting.")]
        public int VotingId { get; set; }

        //Lados varios de la relacion:

       
        public virtual Voting Voting { get; set; }

       
        public virtual Group Group { get; set; }



    }
}
