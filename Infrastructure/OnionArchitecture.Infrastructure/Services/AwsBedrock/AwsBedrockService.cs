using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using OnionArchitecture.Application.Abstractions.Services.AwsBedrock;
using OnionArchitecture.Application.Features.Aws.Command.ClaudThreeSonnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Infrastructure.Services.AwsBedrock
{
    public class AwsBedrockService : IAwsBedrockService
    {
        private static readonly string accessKey = "ASIAUSJEUDTKVYHFZEOU";
        private static readonly string secretKey = "YfIUvgaC/QM7J68Oen3wqABQaSTjy+AIRpgLyK/0";
        private static readonly string sessionToken = "IQoJb3JpZ2luX2VjEHYaCXVzLWVhc3QtMSJHMEUCIQDKl3hbTc0tBxU8B3pYBf14EdO9b+1tiGgVEMtiCqDZ7QIgODDThFKl4efelbc+rAc+Tr5IrnnWy2EeTN67UlwJg88qhAMI7///////////ARAAGgwzMTQxNDYyOTkwOTMiDK3evU+L6wvlmT6bdCrYAgHxAcePq6Sf+s2mHCgYyzlKGPxR5G2Zsyx77eZ5VcQ8L2OI2191/ie37BMThsQuavazooF8VXccSXFp/6qqN4h8sfx5FvnYPrSss0JFgyPO1eFR/DaDwsYWBhhJjgPxd3L+e8jqu6jCGQSRVVBSr03/Ua0wTKn73HbQuJoWUGAUwHgqh47vs8TOrNF/OnF+EuBxsG2PUcREsVCT8ORQyI0sQbSMSdrmGlDrpLDzwWdu/x0Y0L5YiGFIqicYj8YzoGxcQa9ioPup4s6z6NRixJ2zwfwH6dvfEL4RLv9QNSivkwTDec0i3M8CJEi7rnZDOWgqz8/tIZpSuS9Qo0voKPanQhPM3npQgcho913GKZ6f+KIeWas9/JkqLlP9sqlDm8Y8ms90204gdwE2hOxTQNp2n339OOmWYm94Y7kYExZvjMQ3xDnu9dxZIYFr4NE6gItVujH2SN1uMPGlo7kGOqcBYCFhhv+36OOzKHX4IqhJZ0kWLfs1gbVUs2ZCI8V+8asF38qZnhzmb0RAds8mTrbDHfa3YlZxPUqeTA6X+lFxuXkY6xrx2k5TAdwPduEMTwB3CekERRRZ6hyWdkMFheZkc/zIgUTHnGQcVlbLpyoN3meUgGQJK5N603HPkyCjrEJVO9UTNC2jnhvt0jJkPX0I4nOp7pgmPFgPgdzHHtJ87KH65WjALfI=";
        private static readonly string prompt = "Kullanıcının bulunduğu sayfa ve bu sayfada yapmak istediği eylem '..' işaretleri arasında verilmiştir. Kullanıcının bulunduğu sayfanın html yapısını da sana iletiyorum. Kullanıcıya bu sayfayı analiz ederek yapmak istediği eylemde yardımcı olmanı istiyorum. Örnek bir konum vereceğin zaman 'sayfanın sağ üst tarafında' gibi yönlendirmeler yapabilirsin. Tıklayacağı butonun renginden, sayfada bulunan görsel öğelerden bahsedebilirsin. Önemli; Kullanıcıya html vb. yazılımsal bir anlatımda bulunma kullanıcı sadece UI ekranlarını görüp görsel ifadeleri yorumlayabilir. O yüzden sade ve anlaşılır net cevaplar dönmeni istiyorum.";
        private static readonly RegionEndpoint region = RegionEndpoint.EUWest2;

        public async Task<ClaudThreeSonnetCommandResponse> InvokeModelAsync(ClaudThreeSonnetCommandRequest request)
        {
            var credentials = new SessionAWSCredentials(accessKey, secretKey, sessionToken);
            using var client = new AmazonBedrockRuntimeClient(credentials, region);

            var combinedInput = $"{prompt} \nYapmak istediği Eylem: {request.Input} \nKullanıcının bulunduğu sayfa: {request.PageBody}";
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

//todo Buradaki modelleri ve en üstteki keyleri uygun yerlere taşı
public class ClaudeResponse
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Role { get; set; }
    public string Model { get; set; }
    public List<Content> Content { get; set; }
    public Usage Usage { get; set; }
}

public class Content
{
    public string Type { get; set; }
    public string Text { get; set; }
}

public class Usage
{
    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
}
