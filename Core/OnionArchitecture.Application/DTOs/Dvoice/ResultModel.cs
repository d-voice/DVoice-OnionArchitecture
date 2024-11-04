using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.DTOs.Dvoice
{
    public class ResultModel
    {
        public required Stream ResultVoice { get; set; }
    }
}
