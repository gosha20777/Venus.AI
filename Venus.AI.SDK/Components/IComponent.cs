using System;
using System.Collections.Generic;
using System.Text;
using Venus.AI.SDK.Components.Messages;

namespace Venus.AI.SDK.Components
{
    public interface IComponent
    {
        IMessage Process(IMessage message);
    }
}
