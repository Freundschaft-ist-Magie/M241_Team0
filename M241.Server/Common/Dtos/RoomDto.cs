using M241.Server.Data.Models;

namespace M241.Server.Common.Dtos
{
    public class RoomDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Humidity { get; set; }
        public int Temperature { get; set; }
        public int AQI { get; set; }
        public int NO2 { get; set; }
        public int O3 { get; set; }
        public string ClientId { get; set; }
        public ClientDto? Client { get; set; }

        public Room MapToRoom()
        {
            return new Room { Id = Id, Name = Name, Humidity = Humidity, Temperature = Temperature, AQI = AQI, Client = Client?.MapToClient() };
        }
    }
}
