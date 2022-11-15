namespace RabbitStudies.RabbitMq.Settings;

public class RabbitMqSettings
{
    public string HostName { get; set; } = "";
    public string UserName { get; set; } = "";
    public int Port { get; set; }
    public string Password { get; set; } = "";
    public RabbitMqRetrySettings RetrySettings { get; set; } = new();
}
