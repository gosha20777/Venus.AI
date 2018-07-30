using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models
{
    public class UserContext
    {
        public long Id { get; set; }
        public string IntentContext { get; set; }
        public string TalkContext { get; set; }
    }
}
