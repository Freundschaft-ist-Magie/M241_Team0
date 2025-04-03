using M241.Server.Data;
using M241.Server.Data.Models;

namespace M241.Server
{
    public static class SeedData
    {
        public static async Task SeedDb(this AeroSenseDbContext context)
        {
            await Task.Yield();
            return;
        }
    }
}
