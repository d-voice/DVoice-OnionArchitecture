using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Features.Aws.Command.Transcribe
{
    public class TranscribeCommandResponse
    {
        public string Transcription { get; set; }
        public string JobStatus { get; set; }
    }
}
