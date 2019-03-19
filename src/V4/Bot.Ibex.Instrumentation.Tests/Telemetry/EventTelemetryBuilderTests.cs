namespace Bot.Ibex.Instrumentation.Tests.Telemetry
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Extensions;
    using Bot.Ibex.Instrumentation.Telemetry;
    using FluentAssertions;
    using Microsoft.Bot.Schema;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("EventTelemetryBuilder")]
    [Trait("Category", "EventTelemetry")]
    public class EventTelemetryBuilderTests
    {
        private const string FakeActivityType = "FAKE-ACTIVITY-TYPE";

        [Theory(DisplayName = "GIVEN activity type other than Message WHEN Build is invoked THEN event telemetry is being created")]
        [InlineAutoData(ActivityTypes.ConversationUpdate, EventTypes.ConversationUpdate)]
        [InlineAutoData(ActivityTypes.EndOfConversation, EventTypes.ConversationEnded)]
        [InlineAutoData(FakeActivityType, EventTypes.OtherActivity)]
        public void GivenActivityTypeOtherThanMessage_WhenBuildIsInvoked_ThenEventTelemetryIsBeingCreated(
            string activityType,
            string expectedTelemetryName,
            InstrumentationSettings settings,
            IFixture fixture)
        {
            // Arrange
            var activity = new Activity
            {
                Type = activityType,
                ChannelId = fixture.Create<string>(),
                Timestamp = DateTimeOffset.MinValue
            };
            var builder = new EventTelemetryBuilder(activity, settings);
            const int expectedNumberOfTelemetryProperties = 3;

            // Act
            var eventTelemetry = builder.Build();

            // Assert
            eventTelemetry.Name.Should().Be(expectedTelemetryName);
            eventTelemetry.Properties.Count.Should().Be(expectedNumberOfTelemetryProperties);
            eventTelemetry.Properties[BotConstants.TypeProperty].Should().Be(activity.Type);
            eventTelemetry.Properties[BotConstants.TimestampProperty].Should().Be(activity.Timestamp.Value.AsIso8601());
            eventTelemetry.Properties[BotConstants.ChannelProperty].Should().Be(activity.ChannelId);
        }

        [Theory(DisplayName = "GIVEN additional properties WHEN Build is invoked THEN event telemetry with properties is being created")]
        [AutoMockData]
        public void GivenAdditionalProperties_WhenBuildIsInvoked_ThenEventTelemetryWithPropertiesIsBeingCreated(
            IActivity activity,
            InstrumentationSettings settings,
            IDictionary<string, string> properties)
        {
            // Arrange
            var builder = new EventTelemetryBuilder(activity, settings, properties);

            // Act
            var eventTelemetry = builder.Build();

            // Assert
            eventTelemetry.Properties.Should().HaveCountGreaterOrEqualTo(properties.Count);
            eventTelemetry.Properties.Should().Contain(properties);
        }

        [Theory(DisplayName = "GIVEN Message type activity and ReplyToId WHEN Build is invoked THEN event telemetry is being created")]
        [AutoData]
        public void GivenMessageTypeActivityAndReplyToId_WhenBuildIsInvoked_ThenEventTelemetryIsBeingCreated(
            InstrumentationSettings settings,
            IFixture fixture)
        {
            // Arrange
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                ChannelId = fixture.Create<string>(),
                ReplyToId = fixture.Create<string>(),
                Text = fixture.Create<string>(),
                Conversation = new ConversationAccount { Id = fixture.Create<string>() }
            };
            var builder = new EventTelemetryBuilder(activity, settings);
            const int expectedNumberOfTelemetryProperties = 4;
            const string expectedTelemetryName = EventTypes.MessageSent;

            // Act
            var eventTelemetry = builder.Build();

            // Assert
            eventTelemetry.Name.Should().Be(expectedTelemetryName);
            eventTelemetry.Properties.Count.Should().Be(expectedNumberOfTelemetryProperties);
            eventTelemetry.Properties[BotConstants.TypeProperty].Should().Be(activity.Type);
            eventTelemetry.Properties[BotConstants.TextProperty].Should().Be(activity.Text);
            eventTelemetry.Properties[BotConstants.ConversationIdProperty].Should().Be(activity.Conversation.Id);
            eventTelemetry.Properties[BotConstants.ChannelProperty].Should().Be(activity.ChannelId);
        }

        [Theory(DisplayName = "GIVEN Message type activity and omit username setting WHEN Build is invoked THEN event telemetry is being created")]
        [AutoData]
        public void GivenMessageTypeActivityAndOmitUsernameSetting_WhenBuildIsInvoked_ThenEventTelemetryIsBeingCreated(
            IFixture fixture)
        {
            // Arrange
            var settings = new InstrumentationSettings { OmitUsernameFromTelemetry = true };
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                ChannelId = fixture.Create<string>(),
                Text = fixture.Create<string>(),
                Conversation = new ConversationAccount { Id = fixture.Create<string>() },
                From = new ChannelAccount { Id = fixture.Create<string>() }
            };
            var builder = new EventTelemetryBuilder(activity, settings);
            const int expectedNumberOfTelemetryProperties = 5;
            const string expectedTelemetryName = EventTypes.MessageReceived;

            // Act
            var eventTelemetry = builder.Build();

            // Assert
            eventTelemetry.Name.Should().Be(expectedTelemetryName);
            eventTelemetry.Properties.Count.Should().Be(expectedNumberOfTelemetryProperties);
            eventTelemetry.Properties[BotConstants.TypeProperty].Should().Be(activity.Type);
            eventTelemetry.Properties[BotConstants.TextProperty].Should().Be(activity.Text);
            eventTelemetry.Properties[BotConstants.UserIdProperty].Should().Be(activity.From.Id);
            eventTelemetry.Properties[BotConstants.ConversationIdProperty].Should().Be(activity.Conversation.Id);
            eventTelemetry.Properties[BotConstants.ChannelProperty].Should().Be(activity.ChannelId);
        }

        [Theory(DisplayName = "GIVEN Message type activity and no omit username setting WHEN Build is invoked THEN event telemetry is being created")]
        [AutoData]
        public void GivenMessageTypeActivityAndNoOmitUsernameSetting_WhenBuildIsInvoked_ThenEventTelemetryIsBeingCreated(
            IFixture fixture)
        {
            // Arrange
            var settings = new InstrumentationSettings { OmitUsernameFromTelemetry = false };
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                ChannelId = fixture.Create<string>(),
                Text = fixture.Create<string>(),
                Conversation = new ConversationAccount { Id = fixture.Create<string>() },
                From = new ChannelAccount { Id = fixture.Create<string>() }
            };
            var builder = new EventTelemetryBuilder(activity, settings);
            const int expectedNumberOfTelemetryProperties = 6;
            const string expectedTelemetryName = EventTypes.MessageReceived;

            // Act
            var eventTelemetry = builder.Build();

            // Assert
            eventTelemetry.Name.Should().Be(expectedTelemetryName);
            eventTelemetry.Properties.Count.Should().Be(expectedNumberOfTelemetryProperties);
            eventTelemetry.Properties[BotConstants.TypeProperty].Should().Be(activity.Type);
            eventTelemetry.Properties[BotConstants.TextProperty].Should().Be(activity.Text);
            eventTelemetry.Properties[BotConstants.UserIdProperty].Should().Be(activity.From.Id);
            eventTelemetry.Properties[BotConstants.UserNameProperty].Should().Be(activity.From.Name);
            eventTelemetry.Properties[BotConstants.ConversationIdProperty].Should().Be(activity.Conversation.Id);
            eventTelemetry.Properties[BotConstants.ChannelProperty].Should().Be(activity.ChannelId);
        }

        [Theory(DisplayName = "GIVEN empty activity WHEN EventTelemetryBuilder is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyActivity_WhenEventTelemetryBuilderIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const IActivity emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new EventTelemetryBuilder(emptyActivity, settings));
        }

        [Theory(DisplayName = "GIVEN empty settings WHEN EventTelemetryBuilder is constructed THEN exception is being thrown")]
        [AutoMockData]
        public void GivenEmptySettings_WhenEventTelemetryBuilderIsConstructed_ThenExceptionIsBeingThrown(
            IActivity activity)
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new EventTelemetryBuilder(activity, emptySettings));
        }
    }
}
