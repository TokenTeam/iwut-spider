namespace SpiderEngine.TestApi.Controllers;

using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class JsonController : ControllerBase
{
    [HttpGet("user")]
    public IActionResult Test1()
    {
        MyUser[] users = [
            new MyUser("Alice", 20),
            new MyUser("Bob", 30),
            new MyUser("Charlie", 40)
        ];
        return Ok(users);
    }

    [HttpPost("user")]
    public IActionResult Test2([FromBody] MyUser user)
    {
        return Ok(user);
    }

    [HttpPost("redirectTest")]
    public IActionResult RedirectTest([FromQuery] string redirectUrl)
    {
        return Redirect(redirectUrl);
    }

    [HttpPost("movePermanently")]
    public IActionResult MovePermanently([FromQuery] string redirectUrl)
    {
        return RedirectPermanent(redirectUrl);
    }
    public record MyUser(string Name, int Age);
}
