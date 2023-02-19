namespace Identity.Auth.Core.Domain.Exceptions;

using System.Net;

public class IdentityException : CustomException
{
    public IdentityException(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.BadRequest,
        params string[] errors)
        : base(message, statusCode, errors)
    {
    }
}
