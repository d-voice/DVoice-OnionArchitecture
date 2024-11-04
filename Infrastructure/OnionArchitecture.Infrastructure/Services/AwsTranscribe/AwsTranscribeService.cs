using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OnionArchitecture.Application.Abstractions.Services.AwsTranscribe;
using OnionArchitecture.Application.Features.Aws.Command.Transcribe;
using OnionArchitecture.Infrastructure.Configurations;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnionArchitecture.Infrastructure.Services.AwsTranscribe
{
    internal class AwsTranscribeService : IAwsTranscribeService
    {
        private readonly IAmazonTranscribeService _amazonTranscribeService;
        private readonly IAmazonS3 _amazonS3;
        private readonly AwsSettings _awsSettings;

        public AwsTranscribeService(IOptions<AwsSettings> awsSettings)
        {
            _awsSettings = awsSettings.Value;

            var credentials = new SessionAWSCredentials(_awsSettings.AccessKey, _awsSettings.SecretKey, _awsSettings.SessionToken);
            var regionEndpoint = RegionEndpoint.GetBySystemName(_awsSettings.Region);

            _amazonTranscribeService = new AmazonTranscribeServiceClient(credentials, regionEndpoint);
            _amazonS3 = new AmazonS3Client(credentials, regionEndpoint);
        }

        public async Task<TranscribeCommandResponse> TranscribeAudioAsync(TranscribeCommandRequest request)
        {
            try
            {
                var transcribeRequest = new StartTranscriptionJobRequest
                {
                    TranscriptionJobName = Guid.NewGuid().ToString(),
                    LanguageCode = request.LanguageCode,
                    Media = new Media { MediaFileUri = request.AudioFileUrl },
                    OutputBucketName = "dvoicebucket"
                };

                var response = await _amazonTranscribeService.StartTranscriptionJobAsync(transcribeRequest);

                string status = response.TranscriptionJob.TranscriptionJobStatus;
                string transcriptionUri = "";

                while (status == "IN_PROGRESS")
                {
                    await Task.Delay(5000);
                    var getResponse = await _amazonTranscribeService.GetTranscriptionJobAsync(
                        new GetTranscriptionJobRequest
                        {
                            TranscriptionJobName = transcribeRequest.TranscriptionJobName,
                        });

                    status = getResponse.TranscriptionJob.TranscriptionJobStatus;

                    if (status == "COMPLETED")
                    {
                        transcriptionUri = getResponse.TranscriptionJob.Transcript.TranscriptFileUri;
                    }

                }
                if (!string.IsNullOrEmpty(transcriptionUri))
                {
                    var s3Uri = new Uri(transcriptionUri);
                    var bucketName = s3Uri.AbsolutePath.Split('/')[1];
                    var objectKey = s3Uri.AbsolutePath.Split('/')[2];

                    var s3Response = await _amazonS3.GetObjectAsync(new GetObjectRequest
                    {
                        BucketName = bucketName,
                        Key = objectKey
                    });

                    using (var streamReader = new StreamReader(s3Response.ResponseStream))
                    {
                        var jsonContent = await streamReader.ReadToEndAsync();

                        using (JsonDocument doc = JsonDocument.Parse(jsonContent))
                        {
                            var transcriptText = doc.RootElement
                                .GetProperty("results")
                                .GetProperty("transcripts")[0]
                                .GetProperty("transcript")
                                .GetString();

                            return new TranscribeCommandResponse
                            {
                                Transcription = transcriptText,
                                JobStatus = status
                            };
                        }
                    }
                }
                else
                {
                    return new TranscribeCommandResponse
                    {
                        Transcription = "Transcription URI not found.",
                        JobStatus = "Failed"
                    };
                }
            }
            catch (Exception ex)
            {
                return new TranscribeCommandResponse
                {
                    Transcription = ex.Message,
                    JobStatus = "Failed"
                };
            }
        }

        public async Task UploadAudioAsync(IFormFile file)
        {
            try
            {
                var bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_amazonS3, "dvoicebucket");
                if (!bucketExists)
                    throw new Exception($"Bucket dvoicebucket does not exist.");

                var fileKey = $"UserSounds/{file.FileName}";

                using (var inputStream = file.OpenReadStream())
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = "dvoicebucket",
                        Key = fileKey,
                        InputStream = inputStream,
                        ContentType = file.ContentType
                    };
                    await _amazonS3.PutObjectAsync(request);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
