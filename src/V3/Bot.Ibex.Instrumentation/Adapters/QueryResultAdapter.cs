namespace Bot.Ibex.Instrumentation.V3.Adapters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Bot.Ibex.Instrumentation.Common.Instrumentations;
    using Common.Adapters;
    using Common.Models;
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

    public class QueryResultAdapter : IQueryResultAdapter
    {
        private readonly QnAMakerResults queryResult;

        public QueryResultAdapter(QnAMakerResults queryResult)
        {
            this.queryResult = queryResult ?? throw new ArgumentNullException(nameof(queryResult));
        }

        public QueryResult QueryResult => this.ConvertQnAMakerResultsToQueryResult();

        private QueryResult ConvertQnAMakerResultsToQueryResult()
        {
            var topScoreAnswer = this.queryResult.Answers.OrderByDescending(x => x.Score).First();
            var result = new QueryResult
            {
                KnowledgeBaseQuestion =
                    string.Join(QuestionsSeparator.Separator, topScoreAnswer.Questions),
                KnowledgeBaseAnswer = topScoreAnswer.Answer,
                Score = topScoreAnswer.Score.ToString(CultureInfo.InvariantCulture)
            };

            return result;
        }
    }
}
