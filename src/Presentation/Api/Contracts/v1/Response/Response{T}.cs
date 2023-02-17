namespace Identity.Auth.Api.Contracts.v1.Response;

public class Response<T>
{
    public Response()
    {
    }

    public Response(T data)
    {
        Data = data;
    }

    public T Data { get; init; }
}
