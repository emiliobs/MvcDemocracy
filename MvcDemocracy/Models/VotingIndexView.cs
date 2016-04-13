﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MvcDemocracy.Models
{
    [NotMapped]
    public class VotingIndexView : Voting
    {
        public User Winner { get; set; }
    }
}