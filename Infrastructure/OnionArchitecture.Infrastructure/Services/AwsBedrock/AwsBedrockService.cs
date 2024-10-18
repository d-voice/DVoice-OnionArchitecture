using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime;
using Newtonsoft.Json;
using OnionArchitecture.Application.Abstractions.Services.AwsBedrock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnionArchitecture.Infrastructure.Services.AwsBedrock
{
public class AwsBedrockService : IAwsBedrockService
{
    private static readonly string accessKey = "ASIAUSJEUDTK6B54MV36";
    private static readonly string secretKey = "faqCZWxfuu3AjyvN7HOrL8vmTXJSTRgb30/ezQzp";
    private static readonly string sessionToken = "IQoJb3JpZ2luX2VjEOb//////////wEaCXVzLWVhc3QtMSJGMEQCIE43/Dk646/PlQ6pbr5Q2OLwVnflUIYzEOVbSr0z5Oe6AiByysLU6hyjITaoyu0bDPN65mx2N9ykuHU8990XG9GLYCrzAghOEAAaDDMxNDE0NjI5OTA5MyIMZ9gNqqHm4I2QKz8BKtACOFsjBBMrBcgXRw/GJW0mD+PHi0JguxY6Tp3GX1j8sjaZnG9eAvO0I6goAi1rJ4pIhMNcHBegpOaAtF2Ts/rGymuR0zw0JkvKTzjs1vmrrEJNwAy5e/SLPwz3bJSfOBjTIG+v7PtV4PKsTyhLdARwdxeuP0W0aIMlybC/PQiusmpaRNGBQJp+UhSWRV1Aqr9bsc6GnRzKNkcujBkNVQT9a0DRsUFTcOc6PoASkmR8e0c5yPo/hlUmJUJPDvX3waj9HPXdcxgpifTlXRGpigICgSo+dcb1BizmDv/Ergf8VksWGIYROU8DPuVU1YYgwJ/2PdBXAC+wf8/tjp8gZ7RMMG2ONBdmCdFz1zUk7dMqt61K5/064qKRnI70HIt06pWgFiEZ1tO4TRe3GZt2ecwAA+6M3isBDZrXWObJc5jusmJv5OXlp4CnvIaDN6tcX0HMMM+dy7gGOqgB9ldKTfU539QROLpAxO/AAJ8F3fCPL/hmGm00qsM1/zeBTXslOgqbeEQIwu3FmCP+vE+hRof/Y/6pNFjvFHJdpk2blrYaKFtPPBhlse+wbp+Tm009Hq43+MdTxbNvSimyWQtTGj2PO16lQbZ/UaxzV+cV/z/o69KWa+fIjGbgeKBjDCpv5rfTapFJ/vtHXARUcjvaJe9Z8v+o5QzIg+VRAMqTfMZnBFP8";
    private static readonly RegionEndpoint region = RegionEndpoint.EUWest2;

    public async Task<string> InvokeModelAsync(string input, string pageBody)
    {
        var credentials = new SessionAWSCredentials(accessKey, secretKey, sessionToken);
        using var client = new AmazonBedrockRuntimeClient(credentials, region);

        var combinedInput = $"{pageBody}\n{input}";
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

        var request = new InvokeModelRequest
        {
            ModelId = "anthropic.claude-3-sonnet-20240229-v1:0",
            Body = memoryStream,
            ContentType = "application/json",
            Accept = "application/json"
        };

        try
        {
            var response = await client.InvokeModelAsync(request);
            using var reader = new StreamReader(response.Body);
            var responseBody = await reader.ReadToEndAsync();
            
            var claudeResponse = JsonConvert.DeserializeObject<ClaudeResponse>(responseBody);
            if (claudeResponse != null && claudeResponse.Content != null && claudeResponse.Content.Count > 0)
            {
                return claudeResponse.Content[0].Text;
            }
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }

        return "No response from Claude.";
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
