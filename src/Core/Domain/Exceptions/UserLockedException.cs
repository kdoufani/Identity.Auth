namespace Identity.Auth.Core.Domain.Exceptions;

public class UserLockedException : Exception
{
    public UserLockedException(string userId) : base($"userId '{userId}' has been locked.")
    {
    }
}
