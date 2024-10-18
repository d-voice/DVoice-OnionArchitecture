using MediatR;
using Microsoft.AspNetCore.Mvc;
using OnionArchitecture.Application.Features.Aws.Command.ClaudThreeSonnet;

namespace OnionArchitecture.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AwsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AwsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("claude-3-sonnet")]
        public async Task<IActionResult> ClaudThreeSonnet([FromBody] ClaudThreeSonnetCommandRequest claudThreeSonnetCommandRequest)
        {
            ClaudThreeSonnetCommandResponse response = await _mediator.Send(claudThreeSonnetCommandRequest);
            return Ok(response);
        }
    }
}
