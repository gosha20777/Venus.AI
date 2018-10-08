using System;
using System.Collections.Generic;
using System.Text;
using Venus.AI.SDK.Components.Messages;

namespace Venus.AI.SDK.Components
{
    public abstract class BaseSttComponent
    {
        public abstract TextMessage Process(VoiceMessage message);
    }
}
