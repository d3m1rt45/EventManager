namespace KonneyTM.DAL
{
    using KonneyTM.Models;
    using KonneyTM.ViewModels;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using System.Linq;

    public class KonneyContext : DbContext
    {
        public KonneyContext() : base("name=KonneyContext")
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}