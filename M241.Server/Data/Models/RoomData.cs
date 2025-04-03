namespace M241.Server.Data.Models
{
    public class RoomData
    {
        public int Id { get; set; }
        public float Humidity { get; set; }
        public float Temperature { get; set; }
        public float Pressure { get; set; }
        public float Gas { get; set; }
        public DateTime TimeStamp { get; set; }
        public ClientDevice? Client { get; set; }
        public string? ClientId { get; set; }
        public Room? Room { get; set; }
        public int RoomId { get; set; }
    }
}
