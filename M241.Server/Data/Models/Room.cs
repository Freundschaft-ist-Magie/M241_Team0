namespace M241.Server.Data.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Humidity { get; set; }
        public ClientDevice? Client { get; set; }
        public int ClientId { get; set; }
    }
}
