namespace Bot.Ibex.Instrumentation.V3.Tests.ActivityLoggers
{
    using System;
    using Microsoft.Bot.Connector;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using V3.ActivityLoggers;
    using V3.Instrumentations;
    using Xunit;

    [Collection("SentimentDialogActivityLogger")]
    [Trait("Category", "ActivityLoggers")]
    public class SentimentDialogActivityLoggerTests
    {
        [Theory(DisplayName = "GIVEN activity WHEN LogAsync is invoked THEN TrackMessageSentiment is invoked")]
        [AutoMockData]
        public async void GivenActivityWhenLogAsyncIsInvokedThenTrackMessageSentimentIsInvoked(
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
        public void GivenEmptyActivityWhenSentimentDialogActivityLoggerIsCreatedThenExceptionIsBeingThrown()
        {
            // Arrange
            const ISentimentInstrumentation sentimentInstrumentation = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentDialogActivityLogger(sentimentInstrumentation));
        }
    }
}