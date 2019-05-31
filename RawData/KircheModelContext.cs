namespace RawData
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class KircheModelContext : DbContext
    {
        public KircheModelContext()
            : base("name=KircheModelContext")
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Building_Type> Building_Type { get; set; }
        public virtual DbSet<Church> Church { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<District> District { get; set; }
        public virtual DbSet<Implementer> Implementer { get; set; }
        public virtual DbSet<Project_Address> Project_Address { get; set; }
        public virtual DbSet<Project_Type> Project_Type { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<Projects_Project_Type> Projects_Project_Type { get; set; }
        public virtual DbSet<Scope> Scope { get; set; }
        public virtual DbSet<State> State { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>()
                .HasMany(e => e.Project_Address)
                .WithOptional(e => e.Address)
                .HasForeignKey(e => e.address_id);

            modelBuilder.Entity<Building_Type>()
                .HasMany(e => e.Projects)
                .WithOptional(e => e.Building_Type)
                .HasForeignKey(e => e.building_type_id);

            modelBuilder.Entity<Church>()
                .HasMany(e => e.Projects)
                .WithOptional(e => e.Church)
                .HasForeignKey(e => e.church_id);

            modelBuilder.Entity<City>()
                .HasMany(e => e.Address)
                .WithOptional(e => e.City)
                .HasForeignKey(e => e.city_id);

            modelBuilder.Entity<District>()
                .HasMany(e => e.Projects)
                .WithRequired(e => e.District)
                .HasForeignKey(e => e.district_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Implementer>()
                .HasMany(e => e.Projects)
                .WithOptional(e => e.Implementer)
                .HasForeignKey(e => e.implementer_id);

            modelBuilder.Entity<Project_Type>()
                .HasMany(e => e.Projects_Project_Type)
                .WithRequired(e => e.Project_Type)
                .HasForeignKey(e => e.project_type_id)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Projects>()
                .HasMany(e => e.Project_Address)
                .WithOptional(e => e.Projects)
                .HasForeignKey(e => new { e.project_id, e.district_id });

            modelBuilder.Entity<Projects>()
                .HasMany(e => e.Projects_Project_Type)
                .WithRequired(e => e.Projects)
                .HasForeignKey(e => new { e.project_id, e.district_id })
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Scope>()
                .HasMany(e => e.Projects)
                .WithOptional(e => e.Scope)
                .HasForeignKey(e => e.scope_id);

            modelBuilder.Entity<State>()
                .HasMany(e => e.Projects)
                .WithOptional(e => e.State)
                .HasForeignKey(e => e.state_id);
        }
    }
}
