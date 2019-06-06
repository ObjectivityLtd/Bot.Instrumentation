namespace Bot.Instrumentation.V3.Tests.ActivityLoggers
{
    using System;
    using Bot.Instrumentation.V3.ActivityLoggers;
    using Bot.Instrumentation.V3.Instrumentations;
    using Microsoft.Bot.Connector;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("SentimentDialogActivityLogger")]
    [Trait("Category", "ActivityLoggers")]
    public class SentimentDialogActivityLoggerTests
    {
        [Theory(DisplayName = "GIVEN activity WHEN LogAsync is invoked THEN TrackMessageSentiment is invoked")]
        [AutoMockData]
        public async void GIVENActivity_WHENLogAsyncIsInvoked_THENTrackMessageSentimentIsInvoked(
            IActivity activity,
            ISentimentInstrumentation sentimentInstrumentation)
        {
            // Arrange
            var instrumentation = new SentimentDialogActivityLogger(sentimentInstrumentation);

            // Act
            await instrumentation.LogAsync(activity).ConfigureAwait(false);

            // Assert
            Mock.Get(sentimentInstrumentation).Verify(
                ai => ai.TrackMessageSentiment(activity), Times.Once);
        }

        [Fact(DisplayName =
            "GIVEN empty activity WHEN SentimentDialogActivityLogger is created THEN exception is being thrown")]
        public void GIVENEmptyActivity_WHENSentimentDialogActivityLoggerIsCreated_THENExceptionIsBeingThrown()
        {
            // Arrange
            const ISentimentInstrumentation sentimentInstrumentation = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentDialogActivityLogger(sentimentInstrumentation));
        }
    }
}