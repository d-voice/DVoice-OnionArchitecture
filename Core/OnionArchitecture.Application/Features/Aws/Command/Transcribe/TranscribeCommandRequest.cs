using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Features.Aws.Command.Transcribe
{
    public class TranscribeCommandRequest : IRequest<TranscribeCommandResponse>
    {
        public string AudioFileUrl { get; set; }
        public string LanguageCode { get; set; } = "tr-TR";
    }
}
