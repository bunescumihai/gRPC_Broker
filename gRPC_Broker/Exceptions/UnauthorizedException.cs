namespace Brocker.Exceptions;

public class UnauthorizedException: Exception
{
    public UnauthorizedException() : base("Unauthorized"){}
}