using MediatR;
using OnionArchitecture.Application.Abstractions.Services.AwsBedrock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Features.Aws.Command.ClaudThreeSonnet
{
    public class ClaudThreeSonnetCommandHandler : IRequestHandler<ClaudThreeSonnetCommandRequest, ClaudThreeSonnetCommandResponse>
    {
        private readonly IAwsBedrockService _awsBedrockService;

        public ClaudThreeSonnetCommandHandler(IAwsBedrockService awsBedrockService)
        {
            _awsBedrockService = awsBedrockService;
        }

        public async Task<ClaudThreeSonnetCommandResponse> Handle(ClaudThreeSonnetCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _awsBedrockService.InvokeModelAsync(request.Input, request.PageBody);
            return new()
            {
                Answer = response
            };
        }
    }
}
