using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
  public  class Group
    {
        /// <summary>
        /// Gets or sets the group Id:
        /// </summary>

        [Key]
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the group description:
        /// </summary>

        [StringLength(50, ErrorMessage = ("The field {0} can contain maximun {1} an minimum {2} character"), MinimumLength = 3)]
        [Required(ErrorMessage = "The field {0} is Requered")]
        public string Description { get; set; }

        //Relación unica:
        [JsonIgnore]
        public virtual ICollection<GroupMember> GroupMembers { get; set; }
        [JsonIgnore]
        public virtual ICollection<VotingGroup> VotingGroups { get; set; }
    }
}
