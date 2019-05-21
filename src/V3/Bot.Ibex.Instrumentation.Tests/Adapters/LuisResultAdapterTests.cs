namespace Bot.Ibex.Instrumentation.V3.Tests.Adapters
{
    using System;
    using Bot.Ibex.Instrumentation.V3.Adapters;
    using Microsoft.Bot.Builder.Luis.Models;
    using Newtonsoft.Json;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using Xunit;

    [Collection("LuisResultAdapter")]
    [Trait("Category", "Adapters")]
    public class LuisResultAdapterTests
    {
        [Theory(DisplayName = "GIVEN any result WHEN LuisResultAdapter is invoked THEN result is mapped")]
        [AutoMockData]
        public void GIVENAnyResult_WHENLuisResultAdapterIsInvoked_THENActivityIsMapped(LuisResult result)
        {
            // Arrange
            var adapter = new LuisResultAdapter(result);

            // Act
            var recognizedIntentResult = adapter.ConvertLuisesultToIntentResult();

            // Assert
            Assert.Equal(recognizedIntentResult.Intent, result.TopScoringIntent.Intent);
            Assert.Equal(recognizedIntentResult.Score, result.TopScoringIntent.Score.ToString());
            Assert.Equal(recognizedIntentResult.Entities, JsonConvert.SerializeObject(result.Entities));
        }

        [Fact(DisplayName = "GIVEN empty result WHEN LuisResultAdapter is invoked THEN exception is being thrown")]
        public void GIVENEmptyResult_WHENLuisResultAdapterIsInvoked_THENExceptionIsBeingThrown()
        {
            // Arrange
            const LuisResult emptyResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new LuisResultAdapter(emptyResult));
        }
    }
}
