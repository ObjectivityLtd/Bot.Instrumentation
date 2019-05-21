namespace Bot.Ibex.Instrumentation.V4.Instrumentations
{
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;

    public interface IIntentInstrumentation
    {
        void TrackIntent(IActivity activity, RecognizerResult result);
    }
}