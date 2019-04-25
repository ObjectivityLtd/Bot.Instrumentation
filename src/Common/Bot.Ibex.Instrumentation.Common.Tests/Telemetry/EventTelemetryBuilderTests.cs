namespace Bot.Ibex.Instrumentation.Common.Tests.Telemetry
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Common.Telemetry;
    using FluentAssertions;
    using global::AutoFixture;
    using global::AutoFixture.Xunit2;
    using Models;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Settings;
    using Xunit;

    [Collection("EventTelemetryBuilder")]
    [Trait("Category", "EventTelemetry")]
    public class EventTelemetryBuilderTests
    {
        private const string FakeActivityType = "FAKE-ACTIVITY-TYPE";

        [Theory(DisplayName =
            "GIVEN activity type other than Message WHEN Build is invoked THEN event telemetry is being created")]
        [InlineAutoData(ActivityTypes.ConversationUpdate, EventTypes.ConversationUpdate)]
        [InlineAutoData(ActivityTypes.EndOfConversation, EventTypes.ConversationEnded)]
        [InlineAutoData(FakeActivityType, EventTypes.OtherActivity)]
        public void GIVENActivityTypeOtherThanMessage_WHENBuildIsInvoked_THENEventTelemetryIsBeingCreated(
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
                TimeStampIso8601 = DateTime.MinValue.ToString(CultureInfo.CurrentCulture)
            };
            var builder = new EventTelemetryBuilder(activity, settings);
            const int expectedNumberOfTelemetryProperties = 3;

            // Act
            var eventTelemetry = builder.Build();

            // Assert
            eventTelemetry.Name.Should().Be(expectedTelemetryName);
            eventTelemetry.Properties.Count.Should().Be(expectedNumberOfTelemetryProperties);
            eventTelemetry.Properties[BotConstants.TypeProperty].Should().Be(activity.Type);
            eventTelemetry.Properties[BotConstants.TimestampProperty].Should().Be(activity.TimeStampIso8601);
            eventTelemetry.Properties[BotConstants.ChannelProperty].Should().Be(activity.ChannelId);
        }

        [Theory(DisplayName =
            "GIVEN additional properties WHEN Build is invoked THEN event telemetry with properties is being created")]
        [AutoMockData]
        public void GIVENAdditionalProperties_WHENBuildIsInvoked_THENEventTelemetryWithPropertiesIsBeingCreated(
            IActivityAdapter activity,
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

        [Theory(DisplayName =
            "GIVEN Message type activity and ReplyToId WHEN Build is invoked THEN event telemetry is being created")]
        [AutoData]
        public void GIVENMessageTypeActivityAndReplyToId_WHENBuildIsInvoked_THENEventTelemetryIsBeingCreated(
            InstrumentationSettings settings,
            IFixture fixture)
        {
            // Arrange
            var messageActivity = new MessageActivity { Text = fixture.Create<string>() };
            var channelAccount = new ChannelAccount
            {
                Id = fixture.Create<string>(),
                Name = fixture.Create<string>()
            };
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                ChannelId = fixture.Create<string>(),
                ReplyToId = fixture.Create<string>(),
                MessageActivity = messageActivity,
                TimeStampIso8601 = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                ChannelAccount = channelAccount
            };
            var builder = new EventTelemetryBuilder(activity, settings);
            const int expectedNumberOfTelemetryProperties = 5;
            const string expectedTelemetryName = EventTypes.MessageSent;

            // Act
            var eventTelemetry = builder.Build();

            // Assert
            eventTelemetry.Name.Should().Be(expectedTelemetryName);
            eventTelemetry.Properties.Count.Should().Be(expectedNumberOfTelemetryProperties);
            eventTelemetry.Properties[BotConstants.TypeProperty].Should().Be(activity.Type);
            eventTelemetry.Properties[BotConstants.TextProperty].Should().Be(activity.MessageActivity.Text);
            eventTelemetry.Properties[BotConstants.ChannelProperty].Should().Be(activity.ChannelId);
            eventTelemetry.Properties[BotConstants.TimestampProperty].Should().Be(activity.TimeStampIso8601);
        }

        [Theory(DisplayName =
            "GIVEN Message type activity and omit username setting WHEN Build is invoked THEN event telemetry is being created")]
        [AutoData]
        public void
            GIVENMessageTypeActivityAndOmitUsernameSetting_WHENBuildIsInvoked_THENEventTelemetryIsBeingCreated(
                IFixture fixture)
        {
            // Arrange
            var settings = new InstrumentationSettings { OmitUsernameFromTelemetry = true };
            var messageActivity = new MessageActivity { Text = fixture.Create<string>() };
            var channelAccount = new ChannelAccount
            {
                Id = fixture.Create<string>(),
                Name = fixture.Create<string>()
            };
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                ChannelId = fixture.Create<string>(),
                MessageActivity = messageActivity,
                ChannelAccount = channelAccount
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
            eventTelemetry.Properties[BotConstants.TextProperty].Should().Be(activity.MessageActivity.Text);
            eventTelemetry.Properties[BotConstants.UserIdProperty].Should().Be(activity.ChannelAccount.Id);
            eventTelemetry.Properties[BotConstants.ChannelProperty].Should().Be(activity.ChannelId);
        }

        [Theory(DisplayName =
            "GIVEN Message type activity and no omit username setting WHEN Build is invoked THEN event telemetry is being created")]
        [AutoData]
        public void
            GIVENMessageTypeActivityAndNoOmitUsernameSetting_WHENBuildIsInvoked_THENEventTelemetryIsBeingCreated(
                IFixture fixture)
        {
            // Arrange
            var settings = new InstrumentationSettings { OmitUsernameFromTelemetry = false };
            var messageActivity = new MessageActivity { Text = fixture.Create<string>() };
            var channelAccount = new ChannelAccount
            {
                Id = fixture.Create<string>(),
                Name = fixture.Create<string>()
            };
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                ChannelId = fixture.Create<string>(),
                MessageActivity = messageActivity,
                ChannelAccount = channelAccount
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
            eventTelemetry.Properties[BotConstants.TextProperty].Should().Be(activity.MessageActivity.Text);
            eventTelemetry.Properties[BotConstants.UserIdProperty].Should().Be(activity.ChannelAccount.Id);
            eventTelemetry.Properties[BotConstants.UserNameProperty].Should().Be(activity.ChannelAccount.Name);
            eventTelemetry.Properties[BotConstants.ChannelProperty].Should().Be(activity.ChannelId);
        }

        [Theory(DisplayName =
            "GIVEN empty activity WHEN EventTelemetryBuilder is constructed THEN exception is being thrown")]
        [AutoData]
        public void GIVENEmptyActivity_WHENEventTelemetryBuilderIsConstructed_THENExceptionIsBeingThrown(
            InstrumentationSettings settings)
        {
            // Arrange
            const Bot.Ibex.Instrumentation.Common.Telemetry.IActivityAdapter emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new EventTelemetryBuilder(emptyActivity, settings));
        }

        [Theory(DisplayName =
            "GIVEN empty settings WHEN EventTelemetryBuilder is constructed THEN exception is being thrown")]
        [AutoMockData]
        public void GIVENEmptySettings_WHENEventTelemetryBuilderIsConstructed_THENExceptionIsBeingThrown(
            IActivityAdapter activity)
        {
            // Arrange
            const InstrumentationSettings emptySettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new EventTelemetryBuilder(activity, emptySettings));
        }
    }
}
