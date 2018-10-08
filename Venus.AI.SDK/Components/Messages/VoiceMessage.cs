using System.Linq;
using Venus.AI.SDK.Components.Exceptions;
namespace Venus.AI.SDK.Components.Messages
{
    public class VoiceMessage : BaseMessage
    {
        private byte[] _voiceData;
        public byte[] Vioce
        {
            get { return _voiceData; }
            set
            {
                if (value != null && value.Any())
                    _voiceData = value;
                else
                    throw new InvalidMessageException(Id, "Invalid Voice");
            }
        }
    }
}
