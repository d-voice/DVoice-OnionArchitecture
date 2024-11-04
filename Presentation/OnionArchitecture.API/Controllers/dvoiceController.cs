using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnionArchitecture.Application.DTOs.Dvoice;
using OnionArchitecture.Application.Features.Aws.Command.ClaudThreeSonnet;
using OnionArchitecture.Application.Features.Aws.Command.Polly;
using OnionArchitecture.Application.Features.Aws.Command.Transcribe;
using OnionArchitecture.Application.Features.DVoice.Command;

namespace OnionArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class dvoiceController : ControllerBase
    {
        private readonly IMediator _mediator;
        public dvoiceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("dvoice")]
        public async Task<IActionResult> DvoiceAI([FromBody] DvoiceRequest request)
        {
            try
            {
                Stream respondedToAIVoice = await _mediator.Send(new DVoiceCommandRequest
                {
                    TranscribeCommandRequest = request.TranscribeCommandRequest,
                    Body = request.Body
                });

                var soundResponse = File(respondedToAIVoice, "audio/mpeg", "output.mp3");
                return soundResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}