namespace Objectivity.Bot.Ibex.Instrumentation.Common.Instrumentations
{
    using Telemetry;

    public interface IQnAInstrumentation
    {
        void TrackEvent(IActivity activity, QueryResult queryResult);
    }
}
