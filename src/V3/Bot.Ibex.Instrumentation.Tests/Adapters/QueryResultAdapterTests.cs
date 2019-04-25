namespace Bot.Ibex.Instrumentation.V3.Tests.Adapters
{
    using System;
    using System.Globalization;
    using System.Linq;
    using Bot.Ibex.Instrumentation.Common.Instrumentations;
    using Common.Models;
    using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using V3.Adapters;
    using Xunit;

    [Collection("QueryResultAdapter")]
    [Trait("Category", "Adapters")]
    public class QueryResultAdapterTests
    {
        [Theory(DisplayName = "GIVEN any query WHEN QueryResultAdapter is invoked THEN query result is mapped")]
        [AutoMockData]
        public void GIVENAnyQuery_WHENQueryResultAdapterIsInvoked_THENQueryResultIsMapped(QnAMakerResults queryResult)
        {
            // Arrange
            var adapter = new QueryResultAdapter(queryResult);
            var topScoreAnswer = queryResult.Answers.OrderByDescending(x => x.Score).First();

            // Act
            var convertedQueryResult = adapter.QueryResult;

            // Assert
            Assert.Equal(convertedQueryResult.KnowledgeBaseQuestion, string.Join(QuestionsSeparator.Separator, topScoreAnswer.Questions));
            Assert.Equal(convertedQueryResult.KnowledgeBaseAnswer, topScoreAnswer.Answer);
            Assert.Equal(convertedQueryResult.Score, topScoreAnswer.Score.ToString(CultureInfo.InvariantCulture));
        }

        [Fact(DisplayName = "GIVEN empty result WHEN QueryResultAdapter is invoked THEN exception is being thrown")]
        public void GIVENEmptyResult_WHENQueryResultAdapterIsInvoked_THENExceptionIsBeingThrown()
        {
            // Arrange
            const QnAMakerResults emptyResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new QueryResultAdapter(emptyResult));
        }
    }
}
