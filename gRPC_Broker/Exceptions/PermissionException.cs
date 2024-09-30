namespace Brocker.Exceptions;

public class PermissionException: Exception
{
    public PermissionException(string message): base(message){}
    
    public PermissionException(): base("You do not have permissions to perform this action"){}
}   