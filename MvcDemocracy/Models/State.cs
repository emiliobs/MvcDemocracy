using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
   public  class State
    {

        /// <summary>
        /// Gets or sets the state Id:
        /// </summary>
       
        [Key]
        public int StateId { get; set; }

        /// <summary>
        /// Gets or sets the state description:
        /// </summary>

        [StringLength(50, ErrorMessage =("The field {0} can contain maximun {1} an minimum {2} character"), MinimumLength = 3)]
        [Required(ErrorMessage = "The field {0} is Requered")]
        [Display(Name = "State Description")]
        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Voting> Votings { get; set; }

    }
}
