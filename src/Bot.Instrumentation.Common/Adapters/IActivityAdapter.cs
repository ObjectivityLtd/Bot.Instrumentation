namespace Bot.Instrumentation.Common.Adapters
{
    using Bot.Instrumentation.Common.Models;

    public interface IActivityAdapter
    {
        string TimeStampIso8601 { get; }

        string Type { get; }

        string ChannelId { get; }

        string ReplyToId { get; }

        MessageActivity MessageActivity { get; }

        ChannelAccount ChannelAccount { get; }
    }
}
