using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Venus.AI.WebApi.Models;
using Venus.AI.WebApi.Models.AiServices;
using Venus.AI.WebApi.Models.Requests;
using Venus.AI.WebApi.Models.Respones;

namespace Venus.AI.WebApi.Controllers
{
    /// <summary>
    /// A controller to process requests
    /// </summary>
    [Produces("application/json")]
    [Route("api/request")]
    public class RequestController : Controller
    {
        private SpeechToTextService _speechToTextService;
        private TextProcessingService _textProcessingService;
        private TextToSpeechService _textToSpeechService;
        public RequestController()
        {
            _speechToTextService = new SpeechToTextService();
            _textProcessingService = new TextProcessingService();
            _textToSpeechService = new TextToSpeechService();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]ApiRequest apiRequest)
        {
            try
            {
                //ApiRequest apiRequest = JsonConvert.DeserializeObject<ApiRequest>(requestStr);
                //convert speech to text
                if(apiRequest.GetRequestType() == Enums.RequestType.Voice)
                {
                    _speechToTextService.Initialize(apiRequest.GetLanguage());
                    var textServiceRespone = await _speechToTextService.Invork(new VoiceRequest() { Id = apiRequest.Id, VoiceData = apiRequest.VoiceData });
                    //recognize text and make text answer
                    _textProcessingService.Initialize(apiRequest.GetLanguage());
                    var textProcessingRespone = await _textProcessingService.Invork(new TextRequest() { Id = textServiceRespone.Id, TextData = textServiceRespone.TextData });
                    //convert text to speech
                    _textToSpeechService.Initialize(apiRequest.GetLanguage());
                    var speechServiceRespone = await _textToSpeechService.Invork(new TextRequest() { Id = textProcessingRespone.Id, TextData = textProcessingRespone.TextData });
                    //make a respone
                    ApiRespone apiRespone = new ApiRespone
                    {
                        Id = speechServiceRespone.Id,
                        VoiceData = speechServiceRespone.VoiceData,
                        OuputText = textProcessingRespone.TextData,
                        IntentName = textProcessingRespone.IntentName,
                        Entities = textProcessingRespone.Entities,
                        WayPoint = textProcessingRespone.WayPoint
                    };

                    textServiceRespone = null;
                    textProcessingRespone = null;
                    speechServiceRespone = null;
                    return Ok(apiRespone);
                }
                else
                {
                    //recognize text and make text answer
                    _textProcessingService.Initialize(apiRequest.GetLanguage());
                    var textProcessingRespone = await _textProcessingService.Invork(new TextRequest() { Id = apiRequest.Id, TextData = apiRequest.TextData });
                    //make a respone
                    ApiRespone apiRespone = new ApiRespone
                    {
                        Id = apiRequest.Id.Value,
                        VoiceData = null,
                        OuputText = textProcessingRespone.TextData,
                        IntentName = textProcessingRespone.IntentName,
                        Entities = textProcessingRespone.Entities
                    };

                    textProcessingRespone = null;
                    return Ok(apiRespone);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex);
            }
        }
    }
}