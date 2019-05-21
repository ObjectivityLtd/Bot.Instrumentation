namespace Bot.Ibex.Instrumentation.V3.Tests.Extensions
{
    using System;
    using AutoFixture.Xunit2;
    using Bot.Ibex.Instrumentation.Common.Extensions;
    using FluentAssertions;
    using Xunit;

    [Collection("DateTimeExtensions")]
    [Trait("Category", "Extensions")]
    public class DateTimeExtensionsTests
    {
        [Theory(DisplayName = "GIVEN DateTime WHEN AsIso8601 is invoked THEN expected string value is being returned")]
        [InlineAutoData(2018, 1, 17, 16, 0, 54, 432, "2018-01-17T16:00:54.432Z")]
        [InlineAutoData(1979, 10, 14, 9, 15, 0, 0, "1979-10-14T09:15:00Z")]
        [InlineAutoData(1999, 12, 31, 23, 59, 59, 999, "1999-12-31T23:59:59.999Z")]
        [InlineAutoData(2000, 1, 1, 0, 0, 0, 0, "2000-01-01T00:00:00Z")]
        public void GIVENDateTime_WHENAsIso8601IsInvoked_THENExpectedStringIsBeingReturned(
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
            var dateTime = new DateTime(year, month, day, hour, minute, second, millisecond, DateTimeKind.Utc);

            // Act
            var actual = dateTime.AsIso8601();

            // Assert
            actual.Should().Be(expected);
        }
    }
}