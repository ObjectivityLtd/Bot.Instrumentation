namespace Bot.Ibex.Instrumentation.Common.Tests.Extensions
{
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Extensions;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Bot.Ibex.Instrumentation.Common.Tests.Telemetry;
    using FluentAssertions;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

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
        public void GIVENActivity_WHENIsIncomingMessageIsInvoked_THENExpectedResultIsBeingReturned(
            string type,
            string replyToId,
            bool expectedResult,
            Activity activity)
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
        public void GIVENActivity_WHENToSentimentInputIsInvoked_THENSentimentInputIsBeingReturned(
            IActivityAdapter activity)
        {
            // Arrange
            const int expectedNumberOfDocuments = 1;
            const string expectedSentimentInputId = "1";

            // Act
            var actualResult = activity.ToSentimentInput();

            // Assert
            actualResult.Documents.Should().HaveCount(expectedNumberOfDocuments);
            actualResult.Documents.Should().ContainSingle(i =>
                i.Text == activity.MessageActivity.Text &&
                i.Id == expectedSentimentInputId);
        }

        [Fact(DisplayName = "GIVEN empty Activity WHEN IsIncomingMessage is invoked THEN False is being returned")]
        public void GIVENEmptyActivity_WHENIsIncomingMessageIsInvoked_THENFalseIsBeingReturned()
        {
            // Arrange
            const IActivityAdapter activity = null;
            const bool expectedResult = false;

            // Act
            var actualResult = activity.IsIncomingMessage();

            // Assert
            actualResult.Should().Be(expectedResult);
        }

        [Fact(DisplayName =
            "GIVEN empty Activity WHEN ToSentimentInput is invoked THEN empty SentimentInput is being returned")]
        public void GIVENEmptyActivity_WHENToSentimentInputIsInvoked_THENEmptySentimentInputIsBeingReturned()
        {
            // Arrange
            const IActivityAdapter activity = null;

            // Act
            var actualResult = activity.ToSentimentInput();

            // Assert
            actualResult.Should().BeNull();
        }
    }
}
