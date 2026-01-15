namespace FabricesStoreManagementSystem.Errors;

public class AuthErrors
{
    public static Error WrongUsernameOrPassword =
        new("Auth.Wrong-Username-Or-Password",
            "Username or password is wrong",
            StatusCodes.Status401Unauthorized);
}
