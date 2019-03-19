namespace Bot.Ibex.Instrumentation.Tests.Middleware
{
    using System;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Middleware;
    using Bot.Ibex.Instrumentation.Sentiments;
    using Xunit;

    [Collection("SentimentInstrumentationMiddlewareSettings")]
    [Trait("Category", "Middleware")]
    public class SentimentInstrumentationMiddlewareSettingsTests
    {
        [Theory(DisplayName = "GIVEN empty InstrumentationSettings and any SentimentInstrumentationMiddlewareSettings WHEN SentimentInstrumentationMiddleware is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenEmptyInstrumentationSettingsAndAnySentimentClientSettings_WhenSentimentInstrumentationMiddlewareSettingsIsConstructed_ThenExceptionIsBeingThrown(
            SentimentClientSettings sentimentClientSettings)
        {
            // Arrange
            const InstrumentationSettings emptyInstrumentationSettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentationMiddlewareSettings(emptyInstrumentationSettings, sentimentClientSettings));
        }

        [Theory(DisplayName = "GIVEN empty InstrumentationSettings and any SentimentInstrumentationMiddlewareSettings WHEN SentimentInstrumentationMiddleware is constructed THEN exception is being thrown")]
        [AutoData]
        public void GivenAnyInstrumentationSettingsAndEmptySentimentClientSettings_WhenSentimentInstrumentationMiddlewareSettingsIsConstructed_ThenExceptionIsBeingThrown(
            InstrumentationSettings instrumentationSettings)
        {
            // Arrange
            const SentimentClientSettings emptySentimentClientSettings = null;

            // Act
            // Assert
            Assert.Throws<ArgumentNullException>(() => new SentimentInstrumentationMiddlewareSettings(instrumentationSettings, emptySentimentClientSettings));
        }
    }
}
