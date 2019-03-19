namespace Bot.Ibex.Instrumentation.Telemetry
{
    public static class EventTypes
    {
        public const string ConversationEnded = "MBFEvent.EndConversation";
        public const string ConversationUpdate = "MBFEvent.StartConversation";
        public const string CustomEvent = "MBFEvent.CustomEvent";
        public const string Intent = "MBFEvent.Intent";
        public const string MessageReceived = "MBFEvent.UserMessage";
        public const string MessageSent = "MBFEvent.BotMessage";
        public const string MessageSentiment = "MBFEvent.Sentiment";
        public const string OtherActivity = "MBFEvent.Other";
        public const string QnaEvent = "MBFEvent.QNAEvent";
    }
}