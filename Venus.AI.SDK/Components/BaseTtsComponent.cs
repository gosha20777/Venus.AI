using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Venus.AI.SDK.Components.Messages;

namespace Venus.AI.SDK.Components
{
    abstract class BaseTtsComponent
    {
        public abstract VoiceMessage Process(TextMessage message);
    }
}
