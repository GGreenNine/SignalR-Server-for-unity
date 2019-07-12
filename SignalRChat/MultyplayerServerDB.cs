using System.Data.Entity.ModelConfiguration.Conventions;

namespace SignalRChat
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using ModelsLibrary;

    public partial class MultiplayerServerDB : DbContext
    {
        public MultiplayerServerDB()
            : base("name=MultyplayerServerDB")
        {
        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<SyncObjectModel> Models { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<OneToOneConstraintIntroductionConvention>();

            modelBuilder.Entity<UserModel>()
                .HasOptional(p => p.Rooms)
                .WithMany(t => t.Users)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<SyncObjectModel>()
                .HasOptional(p => p.UserModel)
                .WithMany(t => t.SyncObjectModel)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<RoomModel>()
                .HasMany(p=>p.Models)
                .WithOptional(s=>s.RoomModel)
                .WillCascadeOnDelete(true);
        }
    }
}
