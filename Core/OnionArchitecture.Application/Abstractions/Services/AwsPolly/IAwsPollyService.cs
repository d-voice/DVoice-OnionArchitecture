using Microsoft.AspNetCore.Http;
using OnionArchitecture.Application.Features.Aws.Command.Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Abstractions.Services.AwsPolly
{
    public interface IAwsPollyService
    {
        Task<PollyCommandResponse> ConvertTextToSpeechAsync(PollyCommandRequest request);
    }
}
