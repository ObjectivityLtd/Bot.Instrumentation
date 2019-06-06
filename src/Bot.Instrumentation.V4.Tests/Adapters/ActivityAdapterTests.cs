namespace Bot.Instrumentation.V4.Tests.Adapters
{
    using System;
    using Bot.Instrumentation.V4.Adapters;
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("ActivityAdapter")]
    [Trait("Category", "Adapters")]
    public class ActivityAdapterTests
    {
        [Theory(DisplayName = "GIVEN any activity WHEN ActivityAdapter is invoked THEN activity is mapped")]
        [AutoMockData]
        public void GIVENAnyActivity_WHENActivityAdapterIsInvoked_THENActivityIsMapped(IActivity activity)
        {
            // Arrange
            var adapter = new ActivityAdapter(activity);
            var time = JsonConvert.SerializeObject(activity.Timestamp?.ToUniversalTime());
            time = time.Substring(1, time.Length - 2);

            // Act
            // Assert
            Assert.Equal(adapter.TimeStampIso8601, time);
            Assert.Equal(adapter.Type, activity.Type);
            Assert.Equal(adapter.ChannelId, activity.ChannelId);
            Assert.Equal(adapter.ReplyToId, activity.ReplyToId);
            Assert.Equal(adapter.MessageActivity.Text, activity.AsMessageActivity().Text);
            Assert.Equal(adapter.MessageActivity.Id, activity.AsMessageActivity().Id);
            Assert.Equal(adapter.ChannelAccount.Name, activity.From.Name);
            Assert.Equal(adapter.ChannelAccount.Id, activity.From.Id);
        }

        [Fact(DisplayName = "GIVEN empty activity result WHEN ActivityAdapter is invoked THEN exception is being thrown")]
        public void GIVENEmptyActivity_WHENActivityAdapterIsInvoked_THENExceptionIsBeingThrown()
        {
            // Arrange
            const IActivity emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new ActivityAdapter(emptyActivity));
        }
    }
}
