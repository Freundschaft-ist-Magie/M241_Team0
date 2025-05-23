﻿using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace M241.Server.Data.Models
{
    public class RoomData
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
        public DateTime TimeStampLocal => TimeStamp.ToLocalTime();

        [JsonPropertyName("timeStampUTC")]
        public DateTime TimeStamp { get; set; }

        [JsonPropertyName("room")]
        public Room? Room { get; set; }

        [JsonPropertyName("roomId")]
        public int RoomId { get; set; }

        [JsonPropertyName("isBurning")]
        [NotMapped]
        public bool IsBurning { get; set; }
    }
}
