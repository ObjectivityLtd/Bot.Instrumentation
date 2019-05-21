namespace Bot.Ibex.Instrumentation.Common.Adapters
{
    using Bot.Ibex.Instrumentation.Common.Models;

    public interface ILuisResultAdapter
    {
        IntentResult IntentResult { get; }
    }
}
