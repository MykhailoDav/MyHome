namespace MyHomeApp.Models;

public class SensorData
{
    public float? Temperature { get; set; }
    public float? Pressure { get; set; }
    public float? Humidity { get; set; }
    public long Timestamp { get; set; }
    public string Status { get; set; } = "ok";
    public string? Message { get; set; }
    
    public DateTime DateTime => DateTimeOffset.FromUnixTimeSeconds(Timestamp).LocalDateTime;
    
    public bool IsError => Status?.Equals("error", StringComparison.OrdinalIgnoreCase) ?? false;
    public bool IsOk => Status?.Equals("ok", StringComparison.OrdinalIgnoreCase) ?? false;
}
