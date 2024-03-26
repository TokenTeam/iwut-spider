namespace SpiderEngine.TestApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class FormController : ControllerBase
    {
        [HttpPost("user")]
        public IActionResult Post([FromForm]string name, [FromForm]string age)
        {
            return Ok(new
            {
                Name = name,
                Age = age
            });
        }
    }
}
