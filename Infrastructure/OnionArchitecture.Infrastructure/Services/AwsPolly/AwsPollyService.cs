using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using OnionArchitecture.Application.Abstractions.Services.AwsPolly;
using OnionArchitecture.Application.Features.Aws.Command.Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Infrastructure.Services.AwsPolly
{
    public class AwsPollyService : IAwsPollyService
    {
        private readonly AmazonPollyClient _amazonPollyClient;
        private static readonly string accessKey = "ASIAUSJEUDTKVYHFZEOU";
        private static readonly string secretKey = "YfIUvgaC/QM7J68Oen3wqABQaSTjy+AIRpgLyK/0";
        private static readonly string sessionToken = "IQoJb3JpZ2luX2VjEHYaCXVzLWVhc3QtMSJHMEUCIQDKl3hbTc0tBxU8B3pYBf14EdO9b+1tiGgVEMtiCqDZ7QIgODDThFKl4efelbc+rAc+Tr5IrnnWy2EeTN67UlwJg88qhAMI7///////////ARAAGgwzMTQxNDYyOTkwOTMiDK3evU+L6wvlmT6bdCrYAgHxAcePq6Sf+s2mHCgYyzlKGPxR5G2Zsyx77eZ5VcQ8L2OI2191/ie37BMThsQuavazooF8VXccSXFp/6qqN4h8sfx5FvnYPrSss0JFgyPO1eFR/DaDwsYWBhhJjgPxd3L+e8jqu6jCGQSRVVBSr03/Ua0wTKn73HbQuJoWUGAUwHgqh47vs8TOrNF/OnF+EuBxsG2PUcREsVCT8ORQyI0sQbSMSdrmGlDrpLDzwWdu/x0Y0L5YiGFIqicYj8YzoGxcQa9ioPup4s6z6NRixJ2zwfwH6dvfEL4RLv9QNSivkwTDec0i3M8CJEi7rnZDOWgqz8/tIZpSuS9Qo0voKPanQhPM3npQgcho913GKZ6f+KIeWas9/JkqLlP9sqlDm8Y8ms90204gdwE2hOxTQNp2n339OOmWYm94Y7kYExZvjMQ3xDnu9dxZIYFr4NE6gItVujH2SN1uMPGlo7kGOqcBYCFhhv+36OOzKHX4IqhJZ0kWLfs1gbVUs2ZCI8V+8asF38qZnhzmb0RAds8mTrbDHfa3YlZxPUqeTA6X+lFxuXkY6xrx2k5TAdwPduEMTwB3CekERRRZ6hyWdkMFheZkc/zIgUTHnGQcVlbLpyoN3meUgGQJK5N603HPkyCjrEJVO9UTNC2jnhvt0jJkPX0I4nOp7pgmPFgPgdzHHtJ87KH65WjALfI=";
        private static readonly RegionEndpoint region = RegionEndpoint.EUWest2;
        public AwsPollyService()
        {
            _amazonPollyClient = new AmazonPollyClient(accessKey, secretKey, sessionToken, region);
        }

        public async Task<Stream> ConvertTextToSpeechAsync(PollyCommandRequest request)
        {
            var synthesizeRequest = new SynthesizeSpeechRequest
            {
                OutputFormat = OutputFormat.Mp3,
                Text = request.Text,
                VoiceId = request.VoiceId,
                LanguageCode = request.LanguageCode
            };

            try
            {
                var synthesizerResponse = await _amazonPollyClient.SynthesizeSpeechAsync(synthesizeRequest);
                return synthesizerResponse.AudioStream;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
