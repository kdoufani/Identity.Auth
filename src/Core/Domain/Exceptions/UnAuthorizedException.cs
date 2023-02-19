namespace Identity.Auth.Core.Domain.Exceptions;

using System.Net;

public class UnAuthorizedException : IdentityException
{
    public UnAuthorizedException(string message) : base(message, HttpStatusCode.Unauthorized)
    {
    }
}
