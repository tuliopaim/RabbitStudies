﻿namespace RabbitStudies.RabbitMq;

public class RabbitMessage 
{
    public RabbitMessage(
        string serializedMessage,
        MessageType messageType)
    {
        SerializedMessage = serializedMessage;
        MessageType = messageType;
    }

    public string SerializedMessage { get; private set; }
    public MessageType MessageType { get; private set; }
}
