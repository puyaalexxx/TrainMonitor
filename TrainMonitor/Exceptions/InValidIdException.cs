namespace TrainMonitor.Exceptions;

public sealed class InvalidIdException : Exception
{
    // Constructor with a default message
    public InvalidIdException()
        : base("The provided ID is invalid.")
    {
    }

    // Optional constructor to allow custom messages
    public InvalidIdException(string message)
        : base(message)
    {
    }
}
