namespace SpiderEngine.TestApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]")]
    [ApiController]
    public class HeaderController : ControllerBase
    {
        [HttpGet("echo")]
        public IActionResult Get([FromHeader(Name = "Echo")]string echo)
        {
            this.HttpContext.Response.Headers.Append("Echo", echo);
            return this.Ok();
        }

        [HttpGet("double")]
        public IActionResult Get([FromHeader(Name = "Value1")]string value1, [FromHeader(Name = "Value2")]string value2)
        {
            this.HttpContext.Response.Headers.Append("Value", value1);
            this.HttpContext.Response.Headers.Append("Value", value2);
            return this.Ok();
        }
    }
}
