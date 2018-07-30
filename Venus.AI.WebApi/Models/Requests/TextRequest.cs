using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Exceptions;
using Newtonsoft.Json;

namespace Venus.AI.WebApi.Models.Requests
{
    [JsonObject]
    public class TextRequest : BaseRequest
    {
        private string _textData;
        [JsonProperty("textData")]
        public string TextData
        {
            get { return _textData; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _textData = value;
                else
                    throw new ApiRequestException(Id, new InvalidTextDataException());
                _textData = value;
            }
        }
    }
}
