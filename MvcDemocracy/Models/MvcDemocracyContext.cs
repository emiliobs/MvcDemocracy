using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcDemocracy.Models
{
    class MvcDemocracyContext : DbContext
    {
        public MvcDemocracyContext() : base("DefaultConnection")
        {

        }

        public DbSet<State> States { get; set; }

        public System.Data.Entity.DbSet<MvcDemocracy.Models.Group> Groups { get; set; }
    }
}
