namespace Bot.Ibex.Instrumentation.V4.Tests.Adapters
{
    using System;
    using System.Globalization;
    using Bot.Ibex.Instrumentation.Common.Models;
    using Bot.Ibex.Instrumentation.V4.Adapters;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;
    using QueryResult = Microsoft.Bot.Builder.AI.QnA.QueryResult;

    [Collection("QueryResultAdapter")]
    [Trait("Category", "Adapters")]
    public class QueryResultAdapterTests
    {
        [Theory(DisplayName = "GIVEN any query WHEN QueryResultAdapter is invoked THEN query result is mapped")]
        [AutoMockData]
        public void GIVENAnyQuery_WHENQueryResultAdapterIsInvoked_THENQueryResultIsMapped(QueryResult queryResult)
        {
            // Arrange
            var adapter = new QueryResultAdapter(queryResult);

            // Act
            var convertedQueryResult = adapter.QueryResult;

            // Assert
            Assert.Equal(convertedQueryResult.KnowledgeBaseQuestion, string.Join(QuestionsSeparator.Separator, queryResult.Questions));
            Assert.Equal(convertedQueryResult.KnowledgeBaseAnswer, queryResult.Answer);
            Assert.Equal(convertedQueryResult.Score, queryResult.Score.ToString(CultureInfo.InvariantCulture));
        }

        [Fact(DisplayName = "GIVEN empty result WHEN QueryResultAdapter is invoked THEN exception is being thrown")]
        public void GIVENEmptyResult_WHENQueryResultAdapterIsInvoked_THENExceptionIsBeingThrown()
        {
            // Arrange
            const QueryResult emptyResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new QueryResultAdapter(emptyResult));
        }
    }
}
