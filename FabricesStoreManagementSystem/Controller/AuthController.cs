namespace FabricesStoreManagementSystem.Controller;

[Route("auth")]
[ApiController]
public class AuthController(IUnitOfWork unitOfWork) : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    [HttpPost("login")]
    public async Task<IActionResult> Login
        ([FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.AuthService.Login(request, HttpContext, cancellationToken);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("verify")]
    public async Task<IActionResult> Verify(CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.AuthService.Verify(HttpContext);
        if (result.IsFailure)
            return result.ToProblem();
        return Ok(result.Value);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _unitOfWork.AuthService.Logout(HttpContext);
        return Ok();
    }

}
