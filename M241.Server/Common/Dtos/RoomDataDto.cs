using M241.Server.Data.Models;
using System.Text.Json.Serialization;

namespace M241.Server.Common.Dtos
{
    public class RoomDataDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("humidity")]
        public float Humidity { get; set; }

        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }

        [JsonPropertyName("pressure")]
        public float Pressure { get; set; }

        [JsonPropertyName("gas")]
        public float Gas { get; set; }

        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set; }

        [JsonPropertyName("room")]
        public Room? Room { get; set; }

        [JsonPropertyName("roomId")]
        public int RoomId { get; set; }

        [JsonPropertyName("isBurning")]
        public bool IsBurning { get; set; }

        public static RoomDataDto MapFromRoom(RoomData roomData)
        {
            return new RoomDataDto
            {
                Id = roomData.Id,
                Humidity = roomData.Humidity,
                Pressure = roomData.Pressure,
                Room = roomData.Room,
                Gas = roomData.Gas,
                Temperature = roomData.Temperature,
                TimeStamp = roomData.TimeStamp,
            };
        }

        public static List<RoomDataDto> MapFromRooms(List<RoomData> roomDatas)
        {
            List<RoomDataDto> mappedRooms = new();

            foreach (var roomData in roomDatas) {
                mappedRooms.Add(new RoomDataDto
                {
                    Id = roomData.Id,
                    Humidity = roomData.Humidity,
                    Pressure = roomData.Pressure,
                    Room = roomData.Room,
                    Gas = roomData.Gas,
                    Temperature = roomData.Temperature,
                    TimeStamp = roomData.TimeStamp,
                });
            }

            return mappedRooms;
        }

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
