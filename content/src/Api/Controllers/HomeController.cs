using Microsoft.AspNetCore.Mvc;

namespace Clean.Architecture.Template.Api.Controllers;


[ApiController]
[Route("api")]
public class HomeController : ControllerBase
{
    public IActionResult Get() => Ok("Hello World");
}
