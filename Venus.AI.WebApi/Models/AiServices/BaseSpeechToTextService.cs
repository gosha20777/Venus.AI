using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Requests;
using Venus.AI.WebApi.Models.Respones;
using Venus.AI.WebApi.Models.Utils;

namespace Venus.AI.WebApi.Models.AiServices
{
    public abstract class BaseSpeechToTextService : IService
    {
        public abstract void Initialize(Enums.Language language);
        public abstract Task<TextRespone> Invork(VoiceRequest request);
    }
}
