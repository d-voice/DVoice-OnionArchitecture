using Amazon.S3.Model;
using Amazon.TranscribeService.Model;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnionArchitecture.Application.Abstractions.Messaging;
using OnionArchitecture.Application.Abstractions.Services.AwsTranscribe;
using OnionArchitecture.Application.Features.Aws.Command.Transcribe;

namespace OnionArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranscribeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IAwsTranscribeService _awsTranscribeService;

        public TranscribeController(IMediator mediator, IAwsTranscribeService awsTranscribeService)
        {
            _mediator = mediator;
            _awsTranscribeService = awsTranscribeService;
        }

        [HttpPost("transcribe")]
        public async Task<IActionResult> TranscribeAudio([FromBody] TranscribeCommandRequest request)
        {
            var response = await _mediator.Send(request);
            return Ok(response);
        }

        [HttpPost("uploadVoice")]
        public async Task<string> UploadUserVoiceAsync(IFormFile file)
        {
            return await _awsTranscribeService.UploadAudioAsync(file);
        }
    }
}
