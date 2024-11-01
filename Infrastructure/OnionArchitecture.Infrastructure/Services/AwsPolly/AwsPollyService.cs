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
        private static readonly string accessKey = "ASIAUSJEUDTK323GGDCU";
        private static readonly string secretKey = "XqmIj7ob5Yoqd2FM/PZ9V8zYB4O85VncacImGOMx";
        private static readonly string sessionToken = "IQoJb3JpZ2luX2VjEDQaCXVzLWVhc3QtMSJHMEUCIHuB4U2FRiEEp9p1UQswZQnkhBfOYa163lTfx/iOzZBZAiEAqQMXLdFn2zJCQ2I8MezoDmCWdyiaDDLEyJonSITvC/gqhAMIrP//////////ARAAGgwzMTQxNDYyOTkwOTMiDG3tW0fGYoqXIXW0YyrYAhiZCdn8HPq6n1KcAqZn+orI57VYIII/tvaNGyptvTL8ZniMwTOCE1csf+nBCZp2Yk0Bw7kS0X1WuEpsnXvi+uVtWzv12X5iEwbz2hZ5FWVQMYCw1oITWijo/BKtC73BkCR8vzxLWiUGvHSHtjyaCJNK1giq4eVRZo92XqPSsdgJHWNjEb9SaCP9BkOLZOLawMmsWfxu4MrkEoE7NBZbcf/6xjOd3IXgWS6Nem/w6Uz4Y8S5/bvc23149zGRdaIexnH2DFEZ7cLnKlcDN5ZcRUkEcuoDvVMhHBCGyKbm5XXvxquyCyHp3YsA5fdKhjDz/8OMUHssWoEF7TwkrsXS3HNf2m8Y49Q0TJDjClPWIJnHdoDw5hXFI+KBgcTkeF0c8tO4npDotKdfnkwLcuiIW5zO22tb/ju1tcCGx0VkQy9MlsJ12TSJw4NxKj5QiHjb6INq1ETJmj4dMJvRlLkGOqcBiVTlsZJ9+5HiCZ4iy2NFlUoCoo5LuLb4cZuG5EJ468kgTlXuwVZP+yzkKNK1BqKEP0K1q0gy8jD2Jr5+ikbAkeh/GJet4Tnyc/Fru+TXt1FdOdZnRec6c4kvIAcvzUG4lgH5EV+tKeeGyKGOe2FMQpPAIqR14ksdEclD611PazZTP1XuDDeqH3JYqUo0wI5mutoHmSUqyGMjIF7VBxNTS85fTBhdpqk=";
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
