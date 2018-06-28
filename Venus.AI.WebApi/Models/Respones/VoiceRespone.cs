using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Respones
{
    public class VoiceRespone : BaseRespone
    {
        public byte[] VoiceData { get; set; }
    }
}
