namespace Backend.Exceptions.AIExceptions;

public class NullAnswerException(string message) : Exception(message)
{
}