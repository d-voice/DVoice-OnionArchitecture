using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.DTOs.Dvoice
{
    public class VoiceToAnswerModel : IRequest<Stream>
    {
        public required string WebBody { get; set; }
        public required IFormFile AudioBlob { get; set; }
    }
}
