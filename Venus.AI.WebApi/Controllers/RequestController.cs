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
using Venus.AI.WebApi.Models.Utils;

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
                var time = DateTime.Now;
                //ApiRequest apiRequest = JsonConvert.DeserializeObject<ApiRequest>(requestStr);
                //convert speech to text
                if(apiRequest.GetRequestType() == Enums.RequestType.Voice)
                {
                    Log.LogInformation(apiRequest.Id.Value, 0, this.GetType().ToString(), "processing voice request");

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
                        InputText = textServiceRespone.TextData,
                        OutputText = textProcessingRespone.TextData,
                        IntentName = textProcessingRespone.IntentName,
                        Entities = textProcessingRespone.Entities,
                        WayPoint = textProcessingRespone.WayPoint
                    };

                    textServiceRespone = null;
                    textProcessingRespone = null;
                    speechServiceRespone = null;

                    Log.LogInformation(apiRequest.Id.Value, 0, this.GetType().ToString(), $"voice request processed in {(DateTime.Now - time).TotalMilliseconds} ms");

                    Console.WriteLine("WayPoint:" + apiRespone.WayPoint);

                    return Ok(apiRespone);
                }
                else
                {
                    Log.LogInformation(apiRequest.Id.Value, 0, this.GetType().ToString(), "processing text request");

                    //recognize text and make text answer
                    _textProcessingService.Initialize(apiRequest.GetLanguage());
                    var textProcessingRespone = await _textProcessingService.Invork(new TextRequest() { Id = apiRequest.Id, TextData = apiRequest.TextData });
                    //make a respone
                    ApiRespone apiRespone = new ApiRespone
                    {
                        Id = apiRequest.Id.Value,
                        VoiceData = null,
                        InputText = apiRequest.TextData,
                        OutputText = textProcessingRespone.TextData,
                        IntentName = textProcessingRespone.IntentName,
                        Entities = textProcessingRespone.Entities,
                        WayPoint = textProcessingRespone.WayPoint
                    };

                    textProcessingRespone = null;

                    Log.LogInformation(apiRequest.Id.Value, 0, this.GetType().ToString(), "text request processed");

                    return Ok(apiRespone);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(apiRequest.Id.Value, 0, this.GetType().ToString(), ex.Message);
                Console.WriteLine(ex);
                string outputFailText = "Я не расслышала, что вы сказали. Пожалуйста, повторите ваш запрос";
                if (apiRequest.GetLanguage() == Enums.Language.English)
                    outputFailText = "I did not hear what you said. Please repeat your request";
                _textToSpeechService.Initialize(apiRequest.GetLanguage());
                var speechServiceRespone = await _textToSpeechService.Invork(new TextRequest() { Id = apiRequest.Id, TextData = outputFailText });
                ApiRespone apiRespone = new ApiRespone
                {
                    Id = speechServiceRespone.Id,
                    VoiceData = speechServiceRespone.VoiceData,
                    InputText = "",
                    OutputText = outputFailText,
                    IntentName = "none",
                    Entities = null,
                    WayPoint = null
                };
                speechServiceRespone = null;
                return Ok(apiRespone);
            }
        }
    }
}