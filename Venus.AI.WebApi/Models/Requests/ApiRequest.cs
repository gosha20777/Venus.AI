using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Exceptions;
using static Venus.AI.WebApi.Models.Enums;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.Requests
{
    [JsonObject]
    public class ApiRequest : BaseRequest
    {
        private byte[] _voiceData;
        private string _textData;
        private Language _language;
        private RequestType _requestType;
        [JsonProperty("voiceData")]
        public byte[] VoiceData
        {
            get { return _voiceData; }
            set
            {
                if (_requestType == Enums.RequestType.Voice && value != null && value.Any())
                    _voiceData = value;
                else if (_requestType == Enums.RequestType.Voice)
                    throw new ApiRequestException(Id, new InvalidVoiceDataException());
            }
        }
        [JsonProperty("textData")]
        public string TextData
        {
            get { return _textData; }
            set
            {
                if (_requestType == Enums.RequestType.Text && !string.IsNullOrWhiteSpace(value))
                    _textData = value;
                else if (_requestType == Enums.RequestType.Text)
                    throw new ApiRequestException(Id, new InvalidTextDataException());
                _textData = value;
            }
        }
        [JsonProperty("requestType")]
        public string RequestType
        {
            get
            {
                switch (_requestType)
                {
                    case Enums.RequestType.Voice:
                        return "voice";
                    case Enums.RequestType.Text:
                        return "text";
                    default:
                        throw new ApiRequestException(Id, new Exception($"Invalid RequestType {_requestType.ToString()}"));
                }
            }
            set
            {
                value = value.ToLower();
                switch (value)
                {
                    case "voice":
                        _requestType = Enums.RequestType.Voice;
                        break;
                    case "text":
                        _requestType = Enums.RequestType.Text;
                        break;
                    default:
                        throw new ApiRequestException(Id, new Exception($"Invalid RequestType {value}"));
                }
            }
        }
        [JsonProperty("language")]
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

        public RequestType GetRequestType()
        {
            return _requestType;
        }
    }
}
