using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.SqlServer;
using CodeGolf.Sql.Models;

namespace CodeGolf.Sql
{
    public class CodeGolfDbContext : DbContext 
    {
        public CodeGolfDbContext()
        {
            
        }

        public CodeGolfDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Problem> Problems { get; set; }
        public DbSet<Solution> Solutions { get; set; }
        public DbSet<SolutionComment> SolutionComments { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<TestCase> TestCases { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            //modelBuilder.Entity<Solution>()
            //       .HasRequired(f => f.Author)
            //        .WithRequiredDependent()
            //        .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Solution>()
            //   .HasRequired(f => f.Problem)
            //    .WithRequiredDependent()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Problem>()
            //   .HasRequired(f => f.Author)
            //    .WithRequiredDependent()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<SolutionComment>()
            //   .HasRequired(f => f.Commentor)
            //    .WithRequiredDependent()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<SolutionComment>()
            //   .HasRequired(f => f.Solution)
            //    .WithRequiredDependent()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<TestCase>()
            //   .HasRequired(f => f.Problem)
            //    .WithRequiredDependent()
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<Vote>()
            //   .HasRequired(f => f.Voter)
            //    .WithRequiredDependent()
            //    .WillCascadeOnDelete(false);
        }
    }
}
