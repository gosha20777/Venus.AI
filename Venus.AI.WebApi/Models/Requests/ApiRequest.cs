using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Exceptions;
using static Venus.AI.WebApi.Models.Enums;

namespace Venus.AI.WebApi.Models.Requests
{
    public class ApiRequest : Request
    {
        private byte[] _voiceData;
        private Language _language;

        public byte[] VoiceData
        {
            get { return _voiceData; }
            set
            {
                if (value != null && value.Any())
                    _voiceData = value;
                else
                    throw new ApiRequestException(Id, new InvalidVoiceDataException());
            }
        }

        public string Language
        {
            get
            {
                switch (_language)
                {
                    case Enums.Language.English:
                        return "eng";
                    case Enums.Language.Russian:
                        return "rus";
                    default:
                        throw new ApiRequestException(Id, new InvalidLanguageException(_language.ToString()));
                }
            }
            set
            {
                value = value.ToLower();
                switch (value)
                {
                    case "eng":
                        _language = Enums.Language.English;
                        break;
                    case "rus":
                        _language = Enums.Language.Russian;
                        break;
                    default:
                        throw new ApiRequestException(Id, new InvalidLanguageException(value));
                }
            }
        }

        public Language GetLanguage()
        {
            return _language;
        }
    }
}
