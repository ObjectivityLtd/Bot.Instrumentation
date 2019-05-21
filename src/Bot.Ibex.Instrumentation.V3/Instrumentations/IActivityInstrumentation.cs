namespace Bot.Ibex.Instrumentation.V3.Instrumentations
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;

    public interface IActivityInstrumentation
    {
        Task TrackActivity(IActivity activity);
    }
}
