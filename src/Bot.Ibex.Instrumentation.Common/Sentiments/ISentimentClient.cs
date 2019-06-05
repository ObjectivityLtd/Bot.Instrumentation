﻿namespace Bot.Ibex.Instrumentation.Common.Sentiments
{
    using System;
    using System.Threading.Tasks;
    using Bot.Ibex.Instrumentation.Common.Adapters;

    public interface ISentimentClient : IDisposable
    {
        Task<double?> GetSentiment(IActivityAdapter activity);
    }
}
