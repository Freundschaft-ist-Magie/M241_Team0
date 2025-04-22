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
        ILogger<MqttService> _logger;

        public MqttService(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<MqttService> logger)
        {
            _serviceProvider = serviceProvider;
            _client = new MqttClientFactory().CreateMqttClient();

            _options = new MqttClientOptionsBuilder()
                .WithTcpServer(configuration["MQTT:server"], 1883)
                .WithCredentials(configuration["MQTT:user"], configuration["MQTT:password"])
                .WithProtocolVersion(MQTTnet.Formatter.MqttProtocolVersion.V500)
                .Build();

            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _client.ConnectedAsync += async e =>
            {
                _logger.LogInformation("✅ Connected to MQTT Broker");

                await _client.SubscribeAsync("room/data");
            };

            _client.ApplicationMessageReceivedAsync += async e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                _logger.LogInformation($"📩 MQTT message: {payload}");

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
                    _logger.LogError($"❌ Error: {ex.Message}");
                }
            };

            try
            {
                var response = await _client.ConnectAsync(_options, cancellationToken);
                _logger.LogInformation("MQTT Result {0}", response.ResultCode);
            }
            catch(Exception e)
            {
                _logger.LogError("MQTT connection could not be established. Ex: {0}", e);
            }

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_client.IsConnected)
                await _client.DisconnectAsync();
        }
    }
}
