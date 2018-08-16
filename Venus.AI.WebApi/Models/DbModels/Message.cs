using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Venus.AI.WebApi.Models.DbModels
{
    public class Message
    {
        public long Id { get; set; }
        public long OwnerId { get; set; }
        public string Replic { get; set; }
        public string Language { get; set; }
        public DateTime Time { get; set; }
    }
}
