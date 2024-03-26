namespace SpiderEngine.TestApi.Controllers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class BasicController : ControllerBase
    {
        [HttpGet("200")]
        public IActionResult Ok200()
        {
            return Ok();
        }

        [HttpGet("500")]
        public IActionResult Error500()
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet("404")]
        public IActionResult NotFound404()
        {
            return NotFound();
        }

        [HttpGet("301")]
        public IActionResult Redirect301([FromQuery]string redirectUrl)
        {
            return Redirect(redirectUrl);
        }

        [HttpGet("hello")]
        public IActionResult Hello([FromQuery]string name)
        {
            return Ok($"Hello, {name}!");
        }
    }
}
