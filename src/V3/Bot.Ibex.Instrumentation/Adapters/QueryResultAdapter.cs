namespace Bot.Ibex.Instrumentation.V3.Adapters
{
    using System.Linq;
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Instrumentations;

    public class QueryResultAdapter
    {
        private readonly QnAMakerResults queryResult;

        public QueryResultAdapter(QnAMakerResults queryResult)
        {
            this.queryResult = queryResult;
        }

        public QueryResult ConvertQnAMakerResultsToQueryResult()
        {
            var result = new QueryResult();
            var topScoreAnswer = this.queryResult.Answers.OrderByDescending(x => x.Score).First();

            result.KnowledgeBaseQuestion = topScoreAnswer.Questions.ToString();
            result.KnowledgeBaseAnswer = topScoreAnswer.Answer;
            result.Score = topScoreAnswer.Score;

            return result;
        }
    }
}
