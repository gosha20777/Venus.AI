using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.Respones
{
    public class ApiRespone : BaseRespone
    {
        public byte[] VoiceData { get; set; }
        public string OuputText { get; set; }
        public string IntentName { get; set; }
        public Dictionary<string, string> Entities { get; set; } = new Dictionary<string, string>();
        public string WayPoint { get; set; }
    }
}
