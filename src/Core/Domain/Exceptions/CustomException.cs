namespace Identity.Auth.Core.Domain.Exceptions;

using System.Net;

public class CustomException : Exception
{
    public CustomException(
        string message,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
        params string[] errors) : base(message)
    {
        ErrorMessages = errors;
        StatusCode = statusCode;
    }

    public IEnumerable<string> ErrorMessages { get; protected set; }

    public HttpStatusCode StatusCode { get; protected set; }
}
