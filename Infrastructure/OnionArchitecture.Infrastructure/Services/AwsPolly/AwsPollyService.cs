using Amazon;
using Amazon.Polly;
using Amazon.Polly.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using OnionArchitecture.Application.Abstractions.Services.AwsPolly;
using OnionArchitecture.Application.Features.Aws.Command.Polly;
using OnionArchitecture.Infrastructure.Configurations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace OnionArchitecture.Infrastructure.Services.AwsPolly
{
    public class AwsPollyService : IAwsPollyService
    {
        private readonly AmazonPollyClient _amazonPollyClient;
        private readonly AwsSettings _awsSettings;

        public AwsPollyService(IOptions<AwsSettings> awsSettings)
        {
            _awsSettings = awsSettings.Value;

            var credentials = new SessionAWSCredentials(_awsSettings.AccessKey, _awsSettings.SecretKey, _awsSettings.SessionToken);
            _amazonPollyClient = new AmazonPollyClient(credentials, RegionEndpoint.GetBySystemName(_awsSettings.Region));
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
