using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
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

        //método que evita el borrado en cascada de los datos relacionados en las tablas:
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<State> States { get; set; }

        public DbSet<MvcDemocracy.Models.Group> Groups { get; set; }

        public DbSet<MvcDemocracy.Models.Voting> Votings { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<VotingGroup> VotingGroups { get; set; }
    }
}
