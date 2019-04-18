namespace Bot.Ibex.Instrumentation.V4.Tests.Adapters
{
    using System;
    using Microsoft.Bot.Builder;
    using Newtonsoft.Json;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using V4.Adapters;
    using Xunit;

    [Collection("TurnContextAdapter")]
    [Trait("Category", "Adapters")]
    public class TurnContextAdapterTests
    {
        [Theory(DisplayName = "GIVEN any activity WHEN TurnContextAdapter is invoked THEN activity is mapped")]
        [AutoMockData]
        public void GivenAnyActivityWhenTurnContextAdapterIsInvokedThenActivityIsMapped(ITurnContext activity)
        {
            // Arrange
            var adapter = new TurnContextAdapter(activity);
            var time = JsonConvert.SerializeObject(activity.Activity.Timestamp?.ToUniversalTime());
            time = time.Substring(1, time.Length - 2);

            // Act
            // Assert
            Assert.Equal(adapter.TimeStampIso8601, time);
            Assert.Equal(adapter.Type, activity.Activity.Type);
            Assert.Equal(adapter.ChannelId, activity.Activity.ChannelId);
            Assert.Equal(adapter.ReplyToId, activity.Activity.ReplyToId);
            Assert.Equal(adapter.MessageActivity.Text, activity.Activity.Text);
            Assert.Equal(adapter.MessageActivity.Id, activity.Activity.Id);
            Assert.Equal(adapter.ChannelAccount.Name, activity.Activity.Name);
            Assert.Equal(adapter.ChannelAccount.Id, activity.Activity.Id);
        }

        [Fact(DisplayName = "GIVEN empty activity result WHEN TurnContextAdapter is invoked THEN exception is being thrown")]
        public void GivenEmptyActivityWhenTurnContextAdapterIsInvokedThenExceptionIsBeingThrown()
        {
            // Arrange
            const ITurnContext emptyActivity = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new TurnContextAdapter(emptyActivity));
        }
    }
}
