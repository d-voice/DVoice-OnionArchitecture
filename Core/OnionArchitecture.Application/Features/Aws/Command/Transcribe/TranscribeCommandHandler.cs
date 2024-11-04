using MediatR;
using OnionArchitecture.Application.Abstractions.Services.AwsTranscribe;
using OnionArchitecture.Application.Features.Aws.Command.Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Features.Aws.Command.Transcribe
{
    public class TranscribeCommandHandler: IRequestHandler<TranscribeCommandRequest, TranscribeCommandResponse>
    {
        private readonly IAwsTranscribeService _awsTranscribeService;

        public TranscribeCommandHandler(IAwsTranscribeService awsTranscribeService)
        {
            _awsTranscribeService = awsTranscribeService;
        }

        public Task<TranscribeCommandResponse> Handle(TranscribeCommandRequest request, CancellationToken cancellationToken)
        {
            return _awsTranscribeService.TranscribeAudioAsync(request);
        }
    }
}
