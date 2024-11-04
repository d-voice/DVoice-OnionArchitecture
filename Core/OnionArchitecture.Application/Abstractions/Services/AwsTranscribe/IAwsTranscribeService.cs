using Microsoft.AspNetCore.Http;
using OnionArchitecture.Application.Features.Aws.Command.Transcribe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Abstractions.Services.AwsTranscribe
{
    public interface IAwsTranscribeService
    {
        Task<TranscribeCommandResponse> TranscribeAudioAsync(TranscribeCommandRequest request);
        Task<string> UploadAudioAsync(IFormFile file);
    }
}
