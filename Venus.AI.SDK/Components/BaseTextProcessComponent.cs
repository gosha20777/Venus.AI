using System;
using System.Collections.Generic;
using System.Text;
using Venus.AI.SDK.Components.Messages;

namespace Venus.AI.SDK.Components
{
    abstract class BaseTextProcessComponent
    {
        public abstract TextMessage Process(TextMessage message);
    }
}
