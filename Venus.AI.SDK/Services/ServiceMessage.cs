using System;
using System.Collections.Generic;
using System.Text;
using Venus.AI.SDK.Core.Enums;

namespace Venus.AI.SDK.Services
{
    public class ServiceMessage
    {
        public long Id { get; set; }
        public string Text { get; set; }
        public byte[] Voice { get; set; }
        public string Language { get; set; }
        public string MessageType { get; set; }
        public string IntentName { get; set; } = "input.unknown";
    }
}
