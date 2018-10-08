using Venus.AI.SDK.Components.Exceptions;
using Venus.AI.SDK.Core.Enums;

namespace Venus.AI.SDK.Components.Messages
{
    public abstract class BaseMessage : IMessage
    {
        private long _id;
        public long Id
        {
            get { return _id; }
            set
            {
                if (value > 0)
                    _id = value;
                else
                    throw new InvalidMessageException(value, "Invalid Id");
            }
        }
        public Language Language { get; set; }
    }
}
