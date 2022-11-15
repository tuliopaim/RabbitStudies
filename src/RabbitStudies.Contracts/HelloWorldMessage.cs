namespace RabbitStudies.Contracts;

public class HelloWorldMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public override string ToString()
    {
        return Id.ToString();
    }
}
