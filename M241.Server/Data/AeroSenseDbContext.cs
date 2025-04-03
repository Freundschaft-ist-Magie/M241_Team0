using M241.Server.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace M241.Server.Data
{
    public class AeroSenseDbContext : IdentityDbContext<AppUser>
    {
        public AeroSenseDbContext(DbContextOptions<AeroSenseDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomData> RoomData { get; set; }
        public DbSet <ClientDevice> Clients { get; set; }
    }
}
