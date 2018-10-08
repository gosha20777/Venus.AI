using Venus.AI.SDK.Components.Exceptions;

namespace Venus.AI.SDK.Components.Messages
{
    public class TextMessage : BaseMessage
    {
        private string _textData;
        public string Text
        {
            get { return _textData; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    _textData = value;
                else
                    throw new InvalidMessageException(Id, "Invalid Text");
            }
        }
    }
}