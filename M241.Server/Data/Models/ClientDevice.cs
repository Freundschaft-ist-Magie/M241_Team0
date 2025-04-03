namespace M241.Server.Data.Models
{
    public class ClientDevice
    {
        public string Id { get; set; } = new Guid().ToString();
        public string Name { get; set; } = string.Empty;
        public Room? Room { get; set; }
        public int RoomId { get; set; }
    }
}
