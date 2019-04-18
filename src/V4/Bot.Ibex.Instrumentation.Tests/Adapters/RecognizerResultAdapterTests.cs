namespace Bot.Ibex.Instrumentation.V4.Tests.Adapters
{
    using System;
    using System.Globalization;
    using Microsoft.Bot.Builder;
    using Newtonsoft.Json;
    using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
    using V4.Adapters;
    using Xunit;

    [Collection("RecognizerResultAdapter")]
    [Trait("Category", "Adapters")]
    public class RecognizerResultAdapterTests
    {
        [Theory(DisplayName = "GIVEN any result WHEN RecognizerResultAdapter is invoked THEN result is mapped")]
        [AutoMockData]
        public void GivenAnyResultWhenRecognizerResultAdapterIsInvokedThenActivityIsMapped(RecognizerResult recognizerResult)
        {
            // Arrange
            var adapter = new RecognizerResultAdapter(recognizerResult);
            var topScoringIntent = recognizerResult.GetTopScoringIntent();

            // Act
            var recognizedIntentResult = adapter.ConvertRecognizerResultToRecognizedIntentResult();

            // Assert
            Assert.Equal(recognizedIntentResult.Intent, topScoringIntent.intent);
            Assert.Equal(recognizedIntentResult.Score, topScoringIntent.score.ToString(CultureInfo.InvariantCulture));
            Assert.Equal(recognizedIntentResult.Entities, recognizerResult.Entities.ToString(Formatting.None));
        }

        [Fact(DisplayName = "GIVEN empty result WHEN RecognizerResultAdapter is invoked THEN exception is being thrown")]
        public void GivenEmptyResultWhenRecognizerResultAdaptertIsInvokedThenExceptionIsBeingThrown()
        {
            // Arrange
            const RecognizerResult emptyRecognizerResult = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new RecognizerResultAdapter(emptyRecognizerResult));
        }
    }
}
