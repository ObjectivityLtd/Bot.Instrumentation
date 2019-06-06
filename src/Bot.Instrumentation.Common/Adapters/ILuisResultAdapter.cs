namespace Bot.Instrumentation.Common.Adapters
{
    using Bot.Instrumentation.Common.Models;

    public interface ILuisResultAdapter
    {
        IntentResult IntentResult { get; }
    }
}
