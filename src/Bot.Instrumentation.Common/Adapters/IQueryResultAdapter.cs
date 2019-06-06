namespace Bot.Instrumentation.Common.Adapters
{
    using Bot.Instrumentation.Common.Models;

    public interface IQueryResultAdapter
    {
        QueryResult QueryResult { get; }
    }
}
