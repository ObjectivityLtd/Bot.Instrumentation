﻿namespace Bot.Instrumentation.V4.Adapters
{
    using System;
    using System.Globalization;
    using Bot.Instrumentation.Common.Adapters;
    using Bot.Instrumentation.Common.Models;

    public class QueryResultAdapter : IQueryResultAdapter
    {
        private readonly Microsoft.Bot.Builder.AI.QnA.QueryResult queryResult;

        public QueryResultAdapter(Microsoft.Bot.Builder.AI.QnA.QueryResult queryResult)
        {
            this.queryResult = queryResult ?? throw new ArgumentNullException(nameof(queryResult));
        }

        public QueryResult QueryResult => new QueryResult
        {
            KnowledgeBaseQuestion = string.Join(QuestionsSeparator.Separator, this.queryResult.Questions),
            KnowledgeBaseAnswer = this.queryResult.Answer,
            Score = this.queryResult.Score.ToString(CultureInfo.InvariantCulture),
        };
    }
}
