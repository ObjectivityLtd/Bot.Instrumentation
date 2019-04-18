namespace Bot.Ibex.Instrumentation.V4.Tests.Adapters
{
    using System;
    using System.Globalization;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.AI.QnA;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using V4.Adapters;
    using Xunit;

    [Collection("QueryResultAdapter")]
    [Trait("Category", "Adapters")]
    public class QueryResultAdapterTests
    {
        [Theory(DisplayName = "GIVEN any query WHEN QueryResultAdapter is invoked THEN query result is mapped")]
        [AutoMockData]
        public void GivenAnyQueryWhenQueryResultAdapterIsInvokedThenQueryResultIsMapped(QueryResult queryResult)
        {
            // Arrange
            string questionsSeparator = ",";
            var adapter = new QueryResultAdapter(queryResult);

            // Act
            var convertedQueryResult = adapter.ConvertQnAMakerResultsToQueryResult();

            // Assert
            Assert.Equal(convertedQueryResult.KnowledgeBaseQuestion, string.Join(questionsSeparator, queryResult.Questions));
            Assert.Equal(convertedQueryResult.KnowledgeBaseAnswer, queryResult.Answer);
            Assert.Equal(convertedQueryResult.Score, queryResult.Score.ToString(CultureInfo.InvariantCulture));
        }

        [Fact(DisplayName = "GIVEN empty result WHEN QueryResultAdapter is invoked THEN exception is being thrown")]
        public void GivenEmptyResultWhenQueryResultAdapterIsInvokedThenExceptionIsBeingThrown()
        {
            // Arrange
            const QueryResult emptyResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new QueryResultAdapter(emptyResult));
        }
    }
}
