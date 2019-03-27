namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using Microsoft.Bot.Connector;

    public interface IQnAInstrumentation
    {
        void TrackEvent(IActivity activity, string userQuery, string kbQuestion, string kbAnswer, double score);
    }
}
