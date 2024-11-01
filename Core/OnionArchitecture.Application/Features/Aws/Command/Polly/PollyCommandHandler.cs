using MediatR;
using OnionArchitecture.Application.Abstractions.Services.AwsPolly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Features.Aws.Command.Polly
{
    public class PollyCommandHandler : IRequestHandler<PollyCommandRequest, Stream>
    {
        private readonly IAwsPollyService _awsPollyService;
        public PollyCommandHandler(IAwsPollyService awsPollyService)
        {
            _awsPollyService = awsPollyService;
        }

        public async Task<Stream> Handle(PollyCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _awsPollyService.ConvertTextToSpeechAsync(request);
            return response;
        }
    }
}
