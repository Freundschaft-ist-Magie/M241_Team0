using M241.Server.Data.Models;
using System.Net.Mail;

namespace M241.Server.Common.Dtos
{
    public class CreateRoomDataDto
    {
        public int Id { get; set; }
        public float Humidity { get; set; }
        public float Temperature { get; set; }
        public float Pressure { get; set; }
        public float Gas { get; set; }
        public string MACAddress { get; set; }

        public RoomData MapToRoomData(Room room)
        {
            return new RoomData
            {
                Id = Id,
                Humidity = Humidity,
                Pressure = Pressure,
                Room = room,
                Gas = Gas,
                Temperature = Temperature,
                TimeStamp = DateTime.Now.ToUniversalTime(),
            };
        }
    }
}
