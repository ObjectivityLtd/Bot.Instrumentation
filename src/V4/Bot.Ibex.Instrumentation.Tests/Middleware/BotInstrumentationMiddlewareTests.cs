namespace Bot.Ibex.Instrumentation.Tests.Middleware
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Middleware;
    using Bot.Ibex.Instrumentation.Telemetry;
    using FluentAssertions;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Schema;
    using Moq;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("BotInstrumentationMiddleware")]
    [Trait("Category", "Middleware")]
    public class BotInstrumentationMiddlewareTests
    {
        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        public BotInstrumentationMiddlewareTests()
        {
            var telemetryConfiguration = new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN turn context with any activity WHEN OnTurnAsync is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public async void GivenTurnContextWithAnyActivity_WhenOnTurnAsyncIsInvoked_ThenEventTelemetryIsBeingSent(
            Activity activity,
            ITurnContext turnContext,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new BotInstrumentationMiddleware(this.telemetryClient, settings);
            Mock.Get(turnContext)
                .SetupGet(c => c.Activity)
                .Returns(activity);

            // Act
            await instrumentation.OnTurnAsync(turnContext, null)
                .ConfigureAwait(false);

            // Assert
            this.mockTelemetryChannel.Verify(tc => tc.Send(It.IsAny<EventTelemetry>()), Times.Once);
        }

        [Theory(DisplayName = "GIVEN turn context WHEN SendActivities is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public async void GivenTurnContext_WhenSendActivitiesInvoked_ThenEventTelemetryIsBeingSent(
            InstrumentationSettings settings,
            ITurnContext turnContext,
            IFixture fixture)
        {
            // Arrange
            var instrumentation = new BotInstrumentationMiddleware(this.telemetryClient, settings);
            var activity = new Activity
            {
                Type = ActivityTypes.ConversationUpdate,
                ChannelId = fixture.Create<string>()
            };
            Mock.Get(turnContext)
                .Setup(c => c.OnSendActivities(It.IsAny<SendActivitiesHandler>()))
                .Callback<SendActivitiesHandler>(h => h(null, new List<Activity> { activity }, () => Task.FromResult(Array.Empty<ResourceResponse>())));
            const int expectedNumberOfTelemetryProperties = 2;
            const string expectedTelemetryName = EventTypes.ConversationUpdate;

            // Act
            await instrumentation.OnTurnAsync(turnContext, null)
                .ConfigureAwait(false);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == expectedTelemetryName &&
                    t.Properties.Count == expectedNumberOfTelemetryProperties &&
                    t.Properties[BotConstants.TypeProperty] == activity.Type &&
                    t.Properties[BotConstants.ChannelProperty] == activity.ChannelId)),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN turn context WHEN UpdateActivity is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public async void GivenTurnContext_WhenUpdateActivityInvoked_ThenEventTelemetryIsBeingSent(
            InstrumentationSettings settings,
            ITurnContext turnContext,
            IFixture fixture)
        {
            // Arrange
            var instrumentation = new BotInstrumentationMiddleware(this.telemetryClient, settings);
            var activity = new Activity
            {
                Type = ActivityTypes.ConversationUpdate,
                ChannelId = fixture.Create<string>()
            };
            Mock.Get(turnContext)
                .Setup(c => c.OnUpdateActivity(It.IsAny<UpdateActivityHandler>()))
                .Callback<UpdateActivityHandler>(h => h(null, activity, () =>
                    Task.FromResult((ResourceResponse)null)));
            const int expectedNumberOfTelemetryProperties = 2;
            const string expectedTelemetryName = EventTypes.ConversationUpdate;

            // Act
            await instrumentation.OnTurnAsync(turnContext, null)
                .ConfigureAwait(false);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == expectedTelemetryName &&
                    t.Properties.Count == expectedNumberOfTelemetryProperties &&
                    t.Properties[BotConstants.TypeProperty] == activity.Type &&
                    t.Properties[BotConstants.ChannelProperty] == activity.ChannelId)),
                Times.Once);
        }

        [Theory(DisplayName = "GIVEN next turn WHEN OnTurnAsync is invoked THEN next turn is being invoked")]
        [AutoData]
        public async void GivenNextTurn_WhenOnTurnAsyncIsInvoked_ThenNextTurnIsBeingInvoked(
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new BotInstrumentationMiddleware(this.telemetryClient, settings);
            var turnContext = new Mock<ITurnContext>();
            var nextTurnInvoked = false;

            // Act
            await instrumentation.OnTurnAsync(turnContext.Object, token => Task.Run(() => nextTurnInvoked = true, token))
                .ConfigureAwait(false);

            // Assert
            nextTurnInvoked.Should().Be(true);
        }

        [Theory(DisplayName = "GIVEN empty turn context WHEN OnTurnAsync is invoked THEN exception is being thrown")]
        [AutoData]
        public async void GivenEmptyTurnContext_WhenOnTurnAsyncIsInvoked_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new BotInstrumentationMiddleware(this.telemetryClient, settings);
            const ITurnContext emptyTurnContext = null;
            NextDelegate nextDelegate = Task.FromCanceled;

            // Act
            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => instrumentation.OnTurnAsync(emptyTurnContext, nextDelegate))
                .ConfigureAwait(false);
        }

        [Theory(DisplayName = "GIVEN empty telemetry client WHEN BotInstrumentationMiddleware is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyTelemetryClient_WhenBotInstrumentationMiddlewareIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const TelemetryClient emptyTelemetryClient = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new BotInstrumentationMiddleware(emptyTelemetryClient, settings));
        }

        [Fact(DisplayName = "GIVEN empty settings WHEN BotInstrumentationMiddleware is constructed THEN exception is being thrown")]
        public void GivenEmptySettings_WhenBotInstrumentationMiddlewareIsConstructed_ThenExceptionIsBeingThrown()
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new BotInstrumentationMiddleware(this.telemetryClient, emptySettings));
        }
    }
}
