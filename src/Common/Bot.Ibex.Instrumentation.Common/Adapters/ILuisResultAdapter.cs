namespace Bot.Ibex.Instrumentation.Common.Adapters
{
    using Common.Models;

    public interface ILuisResultAdapter
    {
        IntentResult IntentResult { get; }
    }
}
