using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Infrastructure.Configurations
{
    public class ClaudeResponse
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Role { get; set; }
        public string Model { get; set; }
        public List<Content> Content { get; set; }
        public Usage Usage { get; set; }
    }
}
