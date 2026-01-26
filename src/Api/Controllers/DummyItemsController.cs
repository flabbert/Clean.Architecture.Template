using Clean.Architecture.Template.Api.Extensions;
using Clean.Architecture.Template.Application.Features.DummyItems;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Clean.Architecture.Template.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DummyItemsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateDummyItemCommand command) => await mediator.Send(command).ToHttpResponse();
}
