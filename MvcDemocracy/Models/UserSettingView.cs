using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcDemocracy.Models
{
    [NotMapped]
    public class UserSettingView:User
    {
        [Display(Name = "New Photo")]
        public HttpPostedFileBase NewPhoto { get; set; }
    }
}