using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Exceptions
{
    public class InvalidVoiceDataException : Exception
    {
        public InvalidVoiceDataException() : base("invalid voice data") { }
    }
}
