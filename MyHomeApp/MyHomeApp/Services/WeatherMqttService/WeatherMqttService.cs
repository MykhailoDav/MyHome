using System.Text;
using System.Text.Json;
using MQTTnet;
using MQTTnet.Client;
using MyHomeApp.Constants;

namespace MyHomeApp.Services;

public class WeatherMqttService : IWeatherMqttService
{
    IMqttClient? mqttClient;

    public event EventHandler<SensorData>? OutdoorDataReceived;
    public event EventHandler<SensorData>? IndoorDataReceived;
    public event EventHandler<string>? StatusChanged;

    public bool IsConnected => mqttClient?.IsConnected ?? false;

    public async Task ConnectAsync(CancellationToken token = default)
    {
        try
        {
            token.ThrowIfCancellationRequested();

            var mqttFactory = new MqttFactory();
            mqttClient = mqttFactory.CreateMqttClient();

            var tlsOptions = new MqttClientOptionsBuilderTlsParameters
            {
                UseTls = true,
                SslProtocol = System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls13,

                IgnoreCertificateChainErrors = true,
                IgnoreCertificateRevocationErrors = true,
                AllowUntrustedCertificates = true,

                CertificateValidationHandler = (certContext) =>
                {
                    return true;
                }
            };

            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(MqttConfig.BrokerUrl, MqttConfig.BrokerPort)
                .WithCredentials(MqttConfig.Username, MqttConfig.Password)
                .WithTls(tlsOptions)
                .WithClientId(MqttConfig.ClientId)
                .WithCleanSession()
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
                .WithTimeout(TimeSpan.FromSeconds(10))
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
            mqttClient.ConnectedAsync += OnConnected;
            mqttClient.DisconnectedAsync += OnDisconnected;

            await mqttClient.ConnectAsync(mqttClientOptions, token);
        }
        catch (OperationCanceledException)
        {
            Trace.WriteLine("MQTT connection cancelled");
            throw;
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"MQTT Connect Error: {ex.GetType().Name} - {ex.Message}");
            if (ex.InnerException is not null)
            {
                Trace.WriteLine($"Inner Exception: {ex.InnerException.GetType().Name} - {ex.InnerException.Message}");
            }
            StatusChanged?.Invoke(this, $"Error: {ex.Message}");
            throw;
        }
    }

    async Task OnConnected(MqttClientConnectedEventArgs e)
    {
        Trace.WriteLine("✅ Connected to MQTT broker");

        if (mqttClient is null)
            return;

        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(MqttConfig.Topics.Outdoor)
            .Build());

        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(MqttConfig.Topics.Indoor)
            .Build());

        await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
            .WithTopic(MqttConfig.Topics.Status)
            .Build());

        StatusChanged?.Invoke(this, "Connected");
    }

    async Task OnDisconnected(MqttClientDisconnectedEventArgs e)
    {
        Trace.WriteLine("❌ Disconnected from MQTT broker");
        StatusChanged?.Invoke(this, "Disconnected");

        if (e.ClientWasConnected && mqttClient is not null)
        {
            await Task.Delay(5000);
            try
            {
                // Check if already reconnected before attempting connection
                if (!mqttClient.IsConnected)
                {
                    await mqttClient.ConnectAsync(mqttClient.Options);
                }
                else
                {
                    Trace.WriteLine("Client already reconnected, skipping reconnection attempt");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"MQTT Reconnect Error: {ex.Message}");
            }
        }
    }

    Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
        var topic = e.ApplicationMessage.Topic;
        var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

        Trace.WriteLine($"📨 Topic: {topic}, Payload: {payload}");

        try
        {
            switch (topic)
            {
                case var t when t == MqttConfig.Topics.Outdoor:
                    var outdoorData = JsonSerializer.Deserialize<SensorData>(payload,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (outdoorData is not null)
                    {
                        OutdoorDataReceived?.Invoke(this, outdoorData);
                    }
                    break;

                case var t when t == MqttConfig.Topics.Indoor:
                    var indoorData = JsonSerializer.Deserialize<SensorData>(payload,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (indoorData is not null)
                    {
                        IndoorDataReceived?.Invoke(this, indoorData);
                    }
                    break;

                case var t when t == MqttConfig.Topics.Status:
                    StatusChanged?.Invoke(this, $"Device: {payload}");
                    break;
            }
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"MQTT Message Parse Error - Topic: {topic}, Payload: {payload}, Error: {ex.Message}");
        }

        return Task.CompletedTask;
    }

    public async Task DisconnectAsync(CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (mqttClient?.IsConnected == true)
        {
            await mqttClient.DisconnectAsync(cancellationToken: token);
        }
    }

    public async Task PublishCommandAsync(string command, CancellationToken token = default)
    {
        token.ThrowIfCancellationRequested();

        if (mqttClient?.IsConnected == true)
        {
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(MqttConfig.Topics.Command)
                .WithPayload(command)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            await mqttClient.PublishAsync(message, token);
        }
    }
}
