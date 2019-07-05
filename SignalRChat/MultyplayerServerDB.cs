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
        }
    }
}
