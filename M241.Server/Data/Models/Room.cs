﻿using System.Text.Json.Serialization;

namespace M241.Server.Data.Models
{
    public class Room
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("macAddress")]
        public string MACAddress { get; set; } = string.Empty;
        [JsonPropertyName("isBurning")]
        public bool IsBurning { get; set; }
    }
}
