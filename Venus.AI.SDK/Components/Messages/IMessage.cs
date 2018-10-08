using Venus.AI.SDK.Core.Enums;

namespace Venus.AI.SDK.Components.Messages
{
    public interface IMessage
    {
        long Id { get; set; }
        Language Language { get; set; }
    }
}
