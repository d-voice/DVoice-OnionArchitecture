using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Abstractions.Services.AwsBedrock
{
    public interface IAwsBedrockService
    {
        Task<string> InvokeModelAsync(string input, string pageBody);
    }

}
