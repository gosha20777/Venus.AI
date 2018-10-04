using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Venus.AI.WebApi.Models;
using Venus.AI.WebApi.Models.AiServices;
using Venus.AI.WebApi.Models.DbModels;
using Venus.AI.WebApi.Models.Requests;
using Venus.AI.WebApi.Models.Respones;
using Venus.AI.WebApi.Models.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Venus.AI.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class IntentController : Controller
    {
        private SpeechToTextService _speechToTextService;
        private TextToSpeechService _textToSpeechService;
        public IntentController()
        {
            _speechToTextService = new SpeechToTextService();
            _textToSpeechService = new TextToSpeechService();
        }

        // GET: api/<controller>
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            IntentList intentList = new IntentList();
            try
            {
                var lines = await System.IO.File.ReadAllLinesAsync("intents.txt");
                intentList.Intents = lines.ToList();
                return Ok(intentList);
            }
            catch
            {
                if (!System.IO.File.Exists("intents.txt"))
                    await System.IO.File.WriteAllLinesAsync("intents.txt", new List<string> { "value-1", "value-2"});
                return Ok(intentList);
            }
        }
        
        /*
        // GET api/<controller>/intent
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
        */

        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> PostAsync([FromBody]IntentModificationRequest intentModificationRequest)
        {
            try
            {
                _speechToTextService.Initialize(intentModificationRequest.GetLanguage());
                var textServiceRespone = await _speechToTextService.Invork(new VoiceRequest() { Id = intentModificationRequest.Id, VoiceData = intentModificationRequest.VoiceData });
                PersinalIntentData persinalIntentData = new PersinalIntentData()
                {
                    Id = textServiceRespone.Id
                };
                await persinalIntentData.WriteData(new KeyValuePair<string, string>(intentModificationRequest.IntentName, textServiceRespone.TextData));
                _textToSpeechService.Initialize(intentModificationRequest.GetLanguage());

                string textAns = "";
                if (intentModificationRequest.GetLanguage() == Models.Enums.Language.English)
                    textAns = $"You have personalized command: {intentModificationRequest.IntentName}.";
                else
                    textAns = $"Вы персонализировали команду: {intentModificationRequest.IntentName}.";
                var speechServiceRespone = await _textToSpeechService.Invork(new TextRequest() { Id = textServiceRespone.Id, TextData = textAns });
                IntentModificationRespone intentModificationRespone = new IntentModificationRespone()
                {
                    Id = speechServiceRespone.Id,
                    IntentName = intentModificationRequest.IntentName,
                    VoiceData = speechServiceRespone.VoiceData
                };
                return Ok(intentModificationRespone);
            }
            catch (Exception ex)
            {
                Log.LogError(intentModificationRequest.Id.Value, 0, this.GetType().ToString(), ex.Message);
                Console.WriteLine(ex);
                string outputFailText = "Я не расслышала, вашу команду. Пожалуйста, повторите ваш запрос.";
                if (intentModificationRequest.GetLanguage() == Enums.Language.English)
                    outputFailText = "I did not hear what you said. Please repeat your request.";
                _textToSpeechService.Initialize(intentModificationRequest.GetLanguage());
                var speechServiceRespone = await _textToSpeechService.Invork(new TextRequest() { Id = intentModificationRequest.Id, TextData = outputFailText });
                IntentModificationRespone intentModificationRespone = new IntentModificationRespone
                {
                    Id = speechServiceRespone.Id,
                    VoiceData = speechServiceRespone.VoiceData,
                    IntentName = "none"
                };
                speechServiceRespone = null;
                return Ok(intentModificationRespone);
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {

        }
    }
}
