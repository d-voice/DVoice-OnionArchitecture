using System.Reflection.Metadata;

namespace OnionArchitecture.API.Models
{
    public class VoiceToAnswerModel
    {
        public required string WebBody { get; set; }
        public required IFormFile AudioBlob { get; set; }
    }
}
