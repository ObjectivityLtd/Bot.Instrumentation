﻿namespace Objectivity.Bot.Ibex.Instrumentation.Common.Tests.Instrumentations
{
    using System;
    using System.Globalization;
    using AutoFixture.XUnit2.AutoMoq.Attributes;
    using Common.Instrumentations;
    using Common.Telemetry;
    using Constants;
    using global::AutoFixture.Xunit2;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.ApplicationInsights.Extensibility;
    using Moq;
    using Settings;
    using Xunit;

    [Collection("QnAInstrumentation")]
    [Trait("Category", "Instrumentations")]
    public class QnAInstrumentationTests
    {
        private const string FakeInstrumentationKey = "FAKE-INSTRUMENTATION-KEY";
        private readonly Mock<ITelemetryChannel> mockTelemetryChannel = new Mock<ITelemetryChannel>();
        private readonly TelemetryClient telemetryClient;

        public QnAInstrumentationTests()
        {
            var telemetryConfiguration =
                new TelemetryConfiguration(FakeInstrumentationKey, this.mockTelemetryChannel.Object);
            this.telemetryClient = new TelemetryClient(telemetryConfiguration);
        }

        [Theory(DisplayName = "GIVEN any activity WHEN TrackEvent is invoked THEN event telemetry is being sent")]
        [AutoMockData]
        public void GivenAnyActivityWhenTrackEventIsInvokedThenEventTelemetryIsBeingSent(
            IActivity activity,
            QueryResult queryResult,
            InstrumentationSettings settings)
        {
            // Arrange
            var instrumentation = new QnAInstrumentation();

            // Act
            instrumentation.TrackEvent(activity, queryResult, settings, this.telemetryClient);

            // Assert
            this.mockTelemetryChannel.Verify(
                tc => tc.Send(It.Is<EventTelemetry>(t =>
                    t.Name == EventTypes.QnaEvent &&
                    t.Properties[QnAConstants.UserQuery] == activity.MessageActivity.Text &&
                    t.Properties[QnAConstants.KnowledgeBaseQuestion] == queryResult.KnowledgeBaseQuestion &&
                    t.Properties[QnAConstants.KnowledgeBaseAnswer] == queryResult.KnowledgeBaseAnswer &&
                    t.Properties[QnAConstants.Score] == queryResult.Score.ToString(CultureInfo.InvariantCulture))),
                Times.Once);
        }
    }
}