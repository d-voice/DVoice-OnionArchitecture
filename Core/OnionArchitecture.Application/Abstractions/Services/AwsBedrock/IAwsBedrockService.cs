using OnionArchitecture.Application.Features.Aws.Command.ClaudThreeSonnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Abstractions.Services.AwsBedrock
{
    public interface IAwsBedrockService
    {
        Task<ClaudThreeSonnetCommandResponse> InvokeModelAsync(ClaudThreeSonnetCommandRequest request);
    }

}
