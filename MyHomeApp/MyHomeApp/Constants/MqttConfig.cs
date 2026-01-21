namespace MyHomeApp.Constants;

public static class MqttConfig
{
    public const string BrokerUrl = "0a2cbb2a55b94edca77b664f403a756a.s1.eu.hivemq.cloud";
    public const int BrokerPort = 8883;
    public const string Username = "MyHomeApp";
    public const string Password = "MyHome123";
    public const string ClientId = "MyHomeApp_MAUI";
    
    public static class Topics
    {
        public const string Outdoor = "weather/outdoor";
        public const string Indoor = "weather/indoor";
        public const string Status = "weather/status";
        public const string Command = "weather/command";
    }
}
