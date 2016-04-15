using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
    public class GroupMember
    {
        [Key]
        public int GroupMemberId { get; set; }

        public int UserId { get; set; }

        public int GroupId { get; set; }


        //virtual es para que no vaya a la BD:
        [JsonIgnore]
        public virtual Group Group { get; set; }

        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
