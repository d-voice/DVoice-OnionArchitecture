using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.TranscribeService;
using Amazon.TranscribeService.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnionArchitecture.Application.Abstractions.Services.AwsTranscribe;
using OnionArchitecture.Application.Features.Aws.Command.Transcribe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OnionArchitecture.Infrastructure.Services.AwsTranscribe
{
    internal class AwsTranscribeService : IAwsTranscribeService
    {
        private readonly IAmazonTranscribeService _amazonTranscribeService;
        private readonly IAmazonS3 _amazonS3;
        private static readonly string accessKey = "ASIAUSJEUDTKVYHFZEOU";
        private static readonly string secretKey = "YfIUvgaC/QM7J68Oen3wqABQaSTjy+AIRpgLyK/0";
        private static readonly string sessionToken = "IQoJb3JpZ2luX2VjEHYaCXVzLWVhc3QtMSJHMEUCIQDKl3hbTc0tBxU8B3pYBf14EdO9b+1tiGgVEMtiCqDZ7QIgODDThFKl4efelbc+rAc+Tr5IrnnWy2EeTN67UlwJg88qhAMI7///////////ARAAGgwzMTQxNDYyOTkwOTMiDK3evU+L6wvlmT6bdCrYAgHxAcePq6Sf+s2mHCgYyzlKGPxR5G2Zsyx77eZ5VcQ8L2OI2191/ie37BMThsQuavazooF8VXccSXFp/6qqN4h8sfx5FvnYPrSss0JFgyPO1eFR/DaDwsYWBhhJjgPxd3L+e8jqu6jCGQSRVVBSr03/Ua0wTKn73HbQuJoWUGAUwHgqh47vs8TOrNF/OnF+EuBxsG2PUcREsVCT8ORQyI0sQbSMSdrmGlDrpLDzwWdu/x0Y0L5YiGFIqicYj8YzoGxcQa9ioPup4s6z6NRixJ2zwfwH6dvfEL4RLv9QNSivkwTDec0i3M8CJEi7rnZDOWgqz8/tIZpSuS9Qo0voKPanQhPM3npQgcho913GKZ6f+KIeWas9/JkqLlP9sqlDm8Y8ms90204gdwE2hOxTQNp2n339OOmWYm94Y7kYExZvjMQ3xDnu9dxZIYFr4NE6gItVujH2SN1uMPGlo7kGOqcBYCFhhv+36OOzKHX4IqhJZ0kWLfs1gbVUs2ZCI8V+8asF38qZnhzmb0RAds8mTrbDHfa3YlZxPUqeTA6X+lFxuXkY6xrx2k5TAdwPduEMTwB3CekERRRZ6hyWdkMFheZkc/zIgUTHnGQcVlbLpyoN3meUgGQJK5N603HPkyCjrEJVO9UTNC2jnhvt0jJkPX0I4nOp7pgmPFgPgdzHHtJ87KH65WjALfI=";
        private static readonly RegionEndpoint region = RegionEndpoint.EUWest2;
        public AwsTranscribeService()
        {
            _amazonTranscribeService = new AmazonTranscribeServiceClient(accessKey, secretKey, sessionToken, region);
            _amazonS3 = new AmazonS3Client(accessKey, secretKey, sessionToken, region);
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
                    throw new NotFoundException($"Bucket dvoicebucket does not exist.");

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
