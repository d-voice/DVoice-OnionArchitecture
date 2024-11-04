using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OnionArchitecture.Application.Abstractions.Services.AwsBedrock;
using OnionArchitecture.Application.Features.Aws.Command.ClaudThreeSonnet;
using OnionArchitecture.Infrastructure.Configurations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Infrastructure.Services.AwsBedrock
{
    public class AwsBedrockService : IAwsBedrockService
    {
        private readonly AwsSettings _awsSettings;

        public AwsBedrockService(IOptions<AwsSettings> awsSettings)
        {
            _awsSettings = awsSettings.Value;
        }

        public async Task<ClaudThreeSonnetCommandResponse> InvokeModelAsync(ClaudThreeSonnetCommandRequest request)
        {
            var credentials = new SessionAWSCredentials(_awsSettings.AccessKey, _awsSettings.SecretKey, _awsSettings.SessionToken);
            using var client = new AmazonBedrockRuntimeClient(credentials, RegionEndpoint.EUWest2);

            var combinedInput = $"{_awsSettings.Prompt} \nYapmak istediği Eylem: {request.Input} \nKullanıcının bulunduğu sayfa: {request.PageBody}";
            var requestBody = new
            {
                anthropic_version = "bedrock-2023-05-31",
                max_tokens = 1000,
                messages = new[]
                {
                new
                {
                    role = "user",
                    content = new[]
                    {
                        new
                        {
                            type = "text",
                            text = combinedInput
                        }
                    }
                }
            }
            };

            string jsonBody = JsonConvert.SerializeObject(requestBody);

            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));

            var _request = new InvokeModelRequest
            {
                ModelId = "anthropic.claude-3-sonnet-20240229-v1:0",
                Body = memoryStream,
                ContentType = "application/json",
                Accept = "application/json"
            };

            try
            {
                var response = await client.InvokeModelAsync(_request);
                using var reader = new StreamReader(response.Body);
                var responseBody = await reader.ReadToEndAsync();

                var claudeResponse = JsonConvert.DeserializeObject<ClaudeResponse>(responseBody);
                if (claudeResponse != null && claudeResponse.Content != null && claudeResponse.Content.Count > 0)
                {
                    ClaudThreeSonnetCommandResponse aiResponse = new ClaudThreeSonnetCommandResponse
                    {
                        Answer = claudeResponse.Content[0].Text
                    };
                    return aiResponse;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            throw new Exception("No response from Claude.");
        }
    }
}

