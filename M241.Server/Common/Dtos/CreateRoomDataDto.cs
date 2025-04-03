using M241.Server.Data.Models;

namespace M241.Server.Common.Dtos
{
    public class CreateRoomDataDto
    {
        public int Id { get; set; }
        public float Humidity { get; set; }
        public float Temperature { get; set; }
        public float Pressure { get; set; }
        public float Gas { get; set; }
        public string ClientId { get; set; }
        public int RoomId { get; set; }

        public RoomData MapToRoomData()
        {
            return new RoomData
            {
                Id = Id,
                Humidity = Humidity,
                ClientId = ClientId,
                Pressure = Pressure,
                Gas = Gas,
                RoomId =
                RoomId,
                Temperature = Temperature,
                TimeStamp = DateTime.Now.ToUniversalTime(),
            };
        }
    }
}
