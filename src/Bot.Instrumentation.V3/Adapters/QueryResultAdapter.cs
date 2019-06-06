namespace Bot.Instrumentation.V3.Adapters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Bot.Instrumentation.Common.Adapters;
    using Bot.Instrumentation.Common.Models;
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;

    public class QueryResultAdapter : IQueryResultAdapter
    {
        private readonly QnAMakerResults queryResult;

        public QueryResultAdapter(QnAMakerResults queryResult)
        {
            this.queryResult = queryResult ?? throw new ArgumentNullException(nameof(queryResult));
        }

        public QueryResult QueryResult
        {
            get
            {
                var topScoreAnswer = this.queryResult.Answers.OrderByDescending(x => x.Score).First();
                return new QueryResult
                {
                    KnowledgeBaseQuestion = string.Join(QuestionsSeparator.Separator, topScoreAnswer.Questions),
                    KnowledgeBaseAnswer = topScoreAnswer.Answer,
                    Score = topScoreAnswer.Score.ToString(CultureInfo.InvariantCulture),
                };
            }
        }
    }
}
