namespace Bot.Ibex.Instrumentation.Instumentation
{
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBotFrameworkInstrumentation
    {
        Task TrackActivity(IActivity activity, IBotData botData = null, IDictionary<string, string> customProperties = null);
        void TrackLuisIntent(IActivity activity, LuisResult result);
        void TrackQnaEvent(IActivity activity, string userQuery, string kbQuestion, string kbAnswer, double score);
        void TrackCustomEvent(IActivity activity, string eventName = EventTypes.CustomEvent, IDictionary<string, string> customEventProperties = null);
        void TrackGoalTriggeredEvent(IActivity activity, string goalName, IDictionary<string, string> goalTriggeredEventProperties = null);
    }
}
