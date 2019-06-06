namespace Bot.Instrumentation.V3.Tests.ActivityLoggers
{
    using System;
    using Bot.Instrumentation.V3.ActivityLoggers;
    using Bot.Instrumentation.V3.Instrumentations;
    using Microsoft.Bot.Connector;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("DialogActivityLogger")]
    [Trait("Category", "ActivityLoggers")]
    public class DialogActivityLoggerTests
    {
        [Theory(DisplayName = "GIVEN activity WHEN LogAsync is invoked THEN TrackActivity is invoked")]
        [AutoMockData]
        public async void GIVENActivity_WHENLogAsyncIsInvoked_THENTrackActivityIsInvoked(
            IActivity activity,
            IActivityInstrumentation activityInstrumentation)
        {
            // Arrange
            var instrumentation = new DialogActivityLogger(activityInstrumentation);

            // Act
            await instrumentation.LogAsync(activity).ConfigureAwait(false);

            // Assert
            Mock.Get(activityInstrumentation).Verify(
                ai => ai.TrackActivity(activity), Times.Once);
        }

        [Fact(DisplayName =
            "GIVEN empty activity WHEN DialogActivityLogger is created THEN exception is being thrown")]
        public void GIVENEmptyActivity_WHENDialogActivityLoggerIsCreated_THENExceptionIsBeingThrown()
        {
            // Arrange
            const IActivityInstrumentation emptyActivityInstrumentation = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new DialogActivityLogger(emptyActivityInstrumentation));
        }
    }
}