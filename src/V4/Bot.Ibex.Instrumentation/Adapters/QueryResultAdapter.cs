﻿namespace Bot.Ibex.Instrumentation.V4.Adapters
{
    using System;
    using System.Globalization;
    using Common.Models;

    public class QueryResultAdapter
    {
        public const string QuestionsSeparator = ",";

        private readonly Microsoft.Bot.Builder.AI.QnA.QueryResult queryResult;

        public QueryResultAdapter(Microsoft.Bot.Builder.AI.QnA.QueryResult queryResult)
        {
            this.queryResult = queryResult ?? throw new ArgumentNullException(nameof(queryResult));
        }

        public QueryResult ConvertQnAMakerResultsToQueryResult()
        {
            var result = new QueryResult();

            result.KnowledgeBaseQuestion = string.Join(QuestionsSeparator, this.queryResult.Questions);
            result.KnowledgeBaseAnswer = this.queryResult.Answer;
            result.Score = this.queryResult.Score.ToString(CultureInfo.InvariantCulture);

            return result;
        }
    }
}
