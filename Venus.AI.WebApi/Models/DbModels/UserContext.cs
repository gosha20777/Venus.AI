using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.DbModels
{
    public class UserContext
    {
        public long Id { get; set; }
        public string IntentContext { get; set; } = string.Empty;
        public string TalkContext { get; set; } = string.Empty;
        public int TalkReplicCount { get; set; } = 0;
    }
}
