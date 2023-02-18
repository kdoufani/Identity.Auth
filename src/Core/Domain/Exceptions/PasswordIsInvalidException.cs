namespace Identity.Auth.Core.Domain.Exceptions;

public class PasswordIsInvalidException : Exception
{
    public PasswordIsInvalidException(string message) : base(message)
    {
    }
}
