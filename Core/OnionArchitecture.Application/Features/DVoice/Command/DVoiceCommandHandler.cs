using MediatR;
using OnionArchitecture.Application.Abstractions.Services.AwsBedrock;
using OnionArchitecture.Application.Abstractions.Services.AwsPolly;
using OnionArchitecture.Application.Abstractions.Services.AwsTranscribe;
using OnionArchitecture.Application.Features.Aws.Command.ClaudThreeSonnet;
using OnionArchitecture.Application.Features.Aws.Command.Polly;
using OnionArchitecture.Application.Features.Aws.Command.Transcribe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Application.Features.DVoice.Command
{
    public class DVoiceCommandHandler : IRequestHandler<DVoiceCommandRequest, Stream>
    {
        private readonly IAwsTranscribeService _awsTranscribeService;
        private readonly IAwsBedrockService _awsBedrockService;
        private readonly IAwsPollyService _awsPollyService;

        public DVoiceCommandHandler(IAwsTranscribeService awsTranscribeService, IAwsBedrockService awsBedrockService, IAwsPollyService awsPollyService)
        {
            _awsTranscribeService = awsTranscribeService;
            _awsBedrockService = awsBedrockService;
            _awsPollyService = awsPollyService;
        }

        public async Task<Stream> Handle(DVoiceCommandRequest request, CancellationToken cancellationToken)
        {
            try
            {
                TranscribeCommandResponse textFilefromVoice = await _awsTranscribeService.TranscribeAudioAsync(request.TranscribeCommandRequest);

                ClaudThreeSonnetCommandResponse claudeResponse = await _awsBedrockService.InvokeModelAsync(
                    new ClaudThreeSonnetCommandRequest
                    {
                        Input = textFilefromVoice.Transcription,
                        PageBody = request.Body
                    });

                Stream respondedToAIVoice = await _awsPollyService.ConvertTextToSpeechAsync(
                    new PollyCommandRequest
                    {
                        Text = claudeResponse.Answer,
                        LanguageCode = "tr-TR",
                        VoiceId = "Filiz"
                    });

                return respondedToAIVoice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
