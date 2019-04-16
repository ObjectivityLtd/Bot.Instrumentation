namespace Bot.Ibex.Instrumentation.V4.Adapters
{
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;

    public class Activity : IActivity
    {
        public string TimeStampIso8601 { get; set; }

        public string Type { get; set; }

        public string ChannelId { get; set; }

        public string ReplyToId { get; set; }

        public MessageActivity MessageActivity { get; set; }

        public ChannelAccount ChannelAccount { get; set; }
    }
}
