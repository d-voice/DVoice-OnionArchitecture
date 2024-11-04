using OnionArchitecture.Application.Features.Aws.Command.Transcribe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.DTOs.Dvoice
{
    public class DvoiceRequest
    {
        public TranscribeCommandRequest TranscribeCommandRequest { get; set; }
        public string Body { get; set; }
    }
}
