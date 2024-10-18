using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Features.Aws.Command.ClaudThreeSonnet
{
    public  class ClaudThreeSonnetCommandRequest: IRequest<ClaudThreeSonnetCommandResponse>
    {
        public string Input { get; set; }
        public string PageBody { get; set; }
    }
}
