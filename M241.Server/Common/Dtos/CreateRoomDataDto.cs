using M241.Server.Data.Models;

namespace M241.Server.Common.Dtos
{
    public class CreateRoomDataDto
    {
        public int Id { get; set; }
        public int Humidity { get; set; }
        public int Temperature { get; set; }
        public int AQI { get; set; }
        public int NO2 { get; set; }
        public int O3 { get; set; }
        public string ClientId { get; set; }
        public int RoomId { get; set; }

        public RoomData MapToRoomData()
        {
            return new RoomData
            {
                Id = Id,
                Humidity = Humidity,
                ClientId = ClientId,
                AQI = AQI,
                NO2 = NO2,
                RoomId =
                RoomId,
                O3 = O3,
                Temperature = Temperature,
                TimeStamp = DateTime.Now.ToUniversalTime(),
            };
        }
    }
}
