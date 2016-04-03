using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
    public class GroupDetailsView
    {
        public int GroupId { get; set; }

        /// <summary>
        /// Gets or sets the group description:
        /// </summary>

        //[StringLength(50, ErrorMessage = ("The field {0} can contain maximun {1} an minimum {2} character"), MinimumLength = 3)]
        //[Required(ErrorMessage = "The field {0} is Requered")]
        public string Description { get; set; }

        public List<GroupMember> Members { get; set; }
    }
}
