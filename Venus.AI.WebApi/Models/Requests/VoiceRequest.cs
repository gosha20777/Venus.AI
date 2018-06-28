using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Venus.AI.WebApi.Models.Exceptions;

namespace Venus.AI.WebApi.Models.Requests
{
    public class VoiceRequest : BaseRequest
    {
        private byte[] _voiceData;
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
    }
}
