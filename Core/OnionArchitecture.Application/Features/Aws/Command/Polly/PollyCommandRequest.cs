using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Features.Aws.Command.Polly
{
    public class PollyCommandRequest: IRequest<Stream>
    {
        public string Text { get; set; }
        public string LanguageCode { get; set; } = "tr-TR";
        public string VoiceId { get; set; } = "Filiz";
    }
}
