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
    }
}
