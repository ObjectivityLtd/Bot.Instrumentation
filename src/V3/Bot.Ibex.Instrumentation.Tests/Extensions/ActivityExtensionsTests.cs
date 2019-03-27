namespace Bot.Ibex.Instrumentation.V3.Tests.Extensions
{
    using FluentAssertions;
    using Microsoft.Bot.Connector;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using V3.Extensions;
    using Xunit;

    //using ActivityTypes = Microsoft.Bot.Schema;

    [Collection("ActivityExtensions")]
    [Trait("Category", "Extensions")]
    public class ActivityExtensionsTests
    {
        private const string ValidReplyToId = "SOME-REPLY-TO-ID";

        [Theory(DisplayName =
            "GIVEN Activity WHEN IsIncomingMessage is invoked THEN expected result is being returned")]
        [InlineAutoMockData(ActivityTypes.Message, ValidReplyToId, false)]
        [InlineAutoMockData("MESSAGE", ValidReplyToId, false)] // Case insensitive
        [InlineAutoMockData(ActivityTypes.Message, null, true)]
        [InlineAutoMockData(ActivityTypes.Invoke, ValidReplyToId, false)]
        public void Given_Activity_WhenIsIncomingMessageIsInvoked_ThenExpectedResultIsBeingReturned(
            string type,
            string replyToId,
            bool expectedResult,
            IActivity activity)
        {
            // Arrange
            activity.ReplyToId = replyToId;
            activity.Type = type;

            // Act
            var actualResult = activity.IsIncomingMessage();

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        [Theory(DisplayName = "GIVEN Activity WHEN ToSentimentInput is invoked THEN SentimentInput is being returned")]
        [AutoMockData]
        public void GivenActivity_WhenToSentimentInputIsInvoked_ThenSentimentInputIsBeingReturned(
            IActivity activity)
        {
            // Arrange
            const int expectedNumberOfDocuments = 1;
            const string expectedSentimentInputId = "1";

            // Act
            var actualResult = activity.ToSentimentInput();

            // Assert
            actualResult.Documents.Should().HaveCount(expectedNumberOfDocuments);
            actualResult.Documents.Should().ContainSingle(i =>
                i.Text == activity.AsMessageActivity().Text &&
                i.Id == expectedSentimentInputId);
        }

        [Fact(DisplayName = "GIVEN empty Activity WHEN IsIncomingMessage is invoked THEN False is being returned")]
        public void Given_EmptyActivity_WhenIsIncomingMessageIsInvoked_ThenFalseIsBeingReturned()
        {
            // Arrange
            const IActivity activity = null;
            const bool expectedResult = false;

            // Act
            var actualResult = activity.IsIncomingMessage();

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        [Fact(DisplayName =
            "GIVEN empty Activity WHEN ToSentimentInput is invoked THEN empty SentimentInput is being returned")]
        public void GivenEmptyActivity_WhenToSentimentInputIsInvoked_ThenEmptySentimentInputIsBeingReturned()
        {
            // Arrange
            const IActivity activity = null;

            // Act
            var actualResult = activity.ToSentimentInput();

            // Assert
            actualResult.Should().BeNull();
        }
    }
}