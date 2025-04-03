using M241.Server.Data;
using M241.Server.Data.Models;

namespace M241.Server
{
    public static class SeedData
    {
        public static async Task SeedDb(this AeroSenseDbContext context)
        {
            await SeedRooms(context);
        }

        public static async Task SeedRooms(AeroSenseDbContext context)
        {
            if (context.Rooms.Any()) return;

            var room = new Room()
            {
                Id = 1,
                Name = "Test",
            };

            await context.Rooms.AddAsync(room);
            await context.SaveChangesAsync();
        }
    }
}
