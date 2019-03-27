namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;

    public interface IIntentInstrumentation
    {
        void TrackIntent(IActivity activity, LuisResult result);
    }
}
