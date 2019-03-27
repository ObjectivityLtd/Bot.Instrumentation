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
        public async void GivenActivity_WhenLogAsyncIsInvoked_ThenTrackMessageSentimentIsInvoked(
            IActivity activity,
            ISentimentInstrumentation sentimentInstrumentation)
        {
            // Arrange
            var instrumentation = new SentimentDialogActivityLogger(sentimentInstrumentation);

            // Act
            await instrumentation.LogAsync(activity);

            // Assert
            Mock.Get(sentimentInstrumentation).Verify(
                ai => ai.TrackMessageSentiment(activity), Times.Once);
        }

        [Fact(DisplayName =
            "GIVEN empty activity WHEN SentimentDialogActivityLogger is created THEN exception is being thrown")]
        public void GivenEmptyActivity_WhenSentimentDialogActivityLoggerIsCreated_ThenExceptionIsBeingThrown()
        {
            // Arrange
            const ISentimentInstrumentation sentimentInstrumentation = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentDialogActivityLogger(sentimentInstrumentation));
        }
    }
}