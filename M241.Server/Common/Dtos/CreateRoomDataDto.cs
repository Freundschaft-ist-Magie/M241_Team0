using M241.Server.Data.Models;
using System.Net.Mail;
using System.Text.Json.Serialization;

namespace M241.Server.Common.Dtos
{
    public class CreateRoomDataDto
    {
        public int Id { get; set; }
        [JsonPropertyName("humidity")]
        public float Humidity { get; set; }
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }
        [JsonPropertyName("pressure")]
        public float Pressure { get; set; }
        [JsonPropertyName("gas")]
        public float Gas { get; set; }
        [JsonPropertyName("macAddress")]
        public string MACAddress { get; set; }
        [JsonPropertyName("timestamp")]
        public long TimeStamp { get; set; } = 0;

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

        public RoomData MapToRoomData(Room room, DateTime timeStamp)
        {
            return new RoomData
            {
                Id = Id,
                Humidity = Humidity,
                Pressure = Pressure,
                Room = room,
                Gas = Gas,
                Temperature = Temperature,
                TimeStamp = timeStamp,
            };
        }
    }
}
