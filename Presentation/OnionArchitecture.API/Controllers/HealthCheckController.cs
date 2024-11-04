using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnionArchitecture.API.Models;
using System.Reflection.Metadata;

namespace OnionArchitecture.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {



        [HttpPost]
        public async Task<IActionResult> HealthCheck([FromForm] VoiceToAnswerModel model)
        {
            // WebBody değerini yazdırıyoruz
            Console.WriteLine("WebBody: " + model.WebBody);

            // AudioBlob'u stream olarak okuyup, boyutunu yazdırıyoruz
            if (model.AudioBlob != null)
            {
                using var stream = model.AudioBlob.OpenReadStream();
                Console.WriteLine($"Audio Blob Size: {stream.Length} bytes");
            }

            var result = new ResultModel { ResultVoice = model.AudioBlob };

            return Ok(result);
        }

    }

}