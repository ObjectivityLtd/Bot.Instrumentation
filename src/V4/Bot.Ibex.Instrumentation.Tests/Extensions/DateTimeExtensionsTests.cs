namespace Bot.Ibex.Instrumentation.Tests.Extensions
{
    using System;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Extensions;
    using FluentAssertions;
    using Xunit;

    [Collection("DateTimeExtensions")]
    [Trait("Category", "Extensions")]
    public class DateTimeExtensionsTests
    {
        [Theory(DisplayName = "GIVEN DateTimeOffset WHEN AsIso8601 is invoked THEN expected string value is being returned")]
        [InlineAutoData(2018, 1, 17, 16, 0, 54, 432, "2018-01-17T16:00:54.432+00:00")]
        [InlineAutoData(1979, 10, 14, 9, 15, 0, 0, "1979-10-14T09:15:00+00:00")]
        [InlineAutoData(1999, 12, 31, 23, 59, 59, 999, "1999-12-31T23:59:59.999+00:00")]
        [InlineAutoData(2000, 1, 1, 0, 0, 0, 0, "2000-01-01T00:00:00+00:00")]
        public void Given_DateTimeOffset_WhenAsIso8601IsInvoked_ThenExpectedStringIsBeingReturned(
            int year,
            int month,
            int day,
            int hour,
            int minute,
            int second,
            int millisecond,
            string expected)
        {
            // Arrange
            var dateTimeOffset = new DateTimeOffset(year, month, day, hour, minute, second, millisecond, TimeSpan.Zero);

            // Act
            var actual = dateTimeOffset.AsIso8601();

            // Assert
            actual.Should().Be(expected);
        }
    }
}
