namespace M241.Server.Data.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Humidity { get; set; }
        public int Temperature { get; set; }
        public int AQI { get; set; }
        public int NO2 { get; set; }
        public int O3 { get; set; }
        public ClientDevice? Client { get; set; }
        public int ClientId { get; set; }
    }
}
