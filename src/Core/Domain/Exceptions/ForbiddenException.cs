namespace Identity.Auth.Core.Domain.Exceptions;

using System.Net;

public class ForbiddenException : IdentityException
{
    public ForbiddenException(string message) : base(message, statusCode: HttpStatusCode.Forbidden)
    {
    }
}
