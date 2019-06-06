namespace Bot.Instrumentation.V3.Instrumentations
{
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
    using Microsoft.Bot.Connector;

    public interface IQnAInstrumentation
    {
        void TrackEvent(IActivity activity, QnAMakerResults queryResult);
    }
}
