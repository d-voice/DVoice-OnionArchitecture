using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnionArchitecture.Application.Features.Aws.Command.Polly;
using OnionArchitecture.Infrastructure.Services.AwsPolly;

namespace OnionArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PollyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> ConvertTextToSpeech([FromBody] PollyCommandRequest request)
        {
            Stream response = await _mediator.Send(request);
            var soundResponse = File(response, "audio/mpeg", "output.mp3");
            return soundResponse;
        }
    }
}
