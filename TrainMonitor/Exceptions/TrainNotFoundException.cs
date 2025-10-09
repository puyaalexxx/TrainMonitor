namespace TrainMonitor.Exceptions;

public sealed class TrainNotFoundException : Exception
{
    // Constructor with a default message
    public TrainNotFoundException()
        : base("Train with the provided ID does not exist.")
    {
    }

    // Optional constructor to allow custom messages
    public TrainNotFoundException(string message)
        : base(message)
    {
    }
}
