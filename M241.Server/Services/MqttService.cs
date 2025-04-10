using M241.Server.Common.Dtos;
using M241.Server.Data;
using M241.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using MQTTnet;
using System.Text;
using System.Text.Json;

namespace M241.Server.Services
{
    public class MqttService : IHostedService
    {
        private IMqttClient _client;
        private MqttClientOptions _options;
        private readonly IServiceProvider _serviceProvider;

        public MqttService(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _client = new MqttClientFactory().CreateMqttClient();

            _options = new MqttClientOptionsBuilder()
                .WithTcpServer(configuration["MQTT:server"], 1883)
                .WithCredentials(configuration["MQTT:user"], configuration["MQTT:password"])
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                .Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.ConnectedAsync += async e =>
            {
                Console.WriteLine("✅ Connected to MQTT Broker");

                await _client.SubscribeAsync("room/data");
            };

            _client.ApplicationMessageReceivedAsync += async e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                Console.WriteLine($"📩 MQTT message: {payload}");

                try
                {
                    var roomData = JsonSerializer.Deserialize<CreateRoomDataDto>(payload);

                    if (roomData != null)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var context = scope.ServiceProvider.GetRequiredService<AeroSenseDbContext>();
                        var room = await context.Rooms.FirstOrDefaultAsync(c => c.MACAddress == roomData.MACAddress);
                        if (room is null)
                        {
                            room = new Room()
                            {
                                MACAddress = roomData.MACAddress,
                            };
                        }

                        context.RoomData.Add(roomData.MapToRoomData(room));
                        await context.SaveChangesAsync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error: {ex.Message}");
                }
            };

            var response = await _client.ConnectAsync(_options, cancellationToken);
            Console.WriteLine("MQTT Result {0}", response.ResultCode);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_client.IsConnected)
                await _client.DisconnectAsync();
        }
    }
}
