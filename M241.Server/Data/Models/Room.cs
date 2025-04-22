namespace M241.Server.Data.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string MACAddress { get; set; } = string.Empty;
        public bool IsBurning { get; set; }
    }
}
