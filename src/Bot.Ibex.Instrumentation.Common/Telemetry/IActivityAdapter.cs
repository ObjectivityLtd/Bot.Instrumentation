namespace Bot.Ibex.Instrumentation.Common.Telemetry
{
    using Bot.Ibex.Instrumentation.Common.Models;

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
