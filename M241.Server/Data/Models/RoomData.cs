namespace M241.Server.Data.Models
{
    public class RoomData
    {
        public int Id { get; set; }
        public int Humidity { get; set; }
        public int Temperature { get; set; }
        public int AQI { get; set; }
        public int NO2 { get; set; }
        public int O3 { get; set; }
        public DateTime TimeStamp { get; set; }
        public ClientDevice? Client { get; set; }
        public string? ClientId { get; set; }
        public Room? Room { get; set; }
        public int RoomId { get; set; }
    }
}
