namespace Bot.Ibex.Instrumentation.Common.Telemetry
{
    using Models;

    public interface IActivity
    {
        string TimeStampIso8601{ get; }
        string Type { get; }
        string ChannelId { get; }
        string ReplyToId { get; }
        MessageActivity MessageActivity { get; }
        ChannelAccount ChannelAccount { get; }
    }
}
