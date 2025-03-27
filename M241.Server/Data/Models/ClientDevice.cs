namespace M241.Server.Data.Models
{
    public class ClientDevice
    {
        public int Id { get; set; }
        public string MACAddr { get; set; }
        public string Name { get; set; } = string.Empty;
        public Room? Room { get; set; }
    }
}
