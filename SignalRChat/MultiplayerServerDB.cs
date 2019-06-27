using ModelsLibrary;

namespace SignalRChat
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MultiplayerServerDB : DbContext
    {
        public MultiplayerServerDB()
            : base("name=MultiplayerServerConnectionDB")
        {
        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<RoomModel> Rooms { get; set; }
        public DbSet<SyncObjectModel> Models { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
