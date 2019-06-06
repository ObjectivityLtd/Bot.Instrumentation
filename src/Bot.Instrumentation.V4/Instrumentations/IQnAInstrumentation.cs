namespace Bot.Instrumentation.V4.Instrumentations
{
    using Microsoft.Bot.Builder.AI.QnA;
    using Microsoft.Bot.Schema;

    public interface IQnAInstrumentation
    {
        void TrackEvent(IMessageActivity activity, QueryResult queryResult);
    }
}
