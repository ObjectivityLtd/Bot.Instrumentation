namespace Bot.Ibex.Instrumentation.Extensions
{
    using System;
    using Newtonsoft.Json;

    public static class DateTimeExtensions
    {
        public static string AsIso8601(this DateTimeOffset activity)
        {
            var s = JsonConvert.SerializeObject(activity.ToUniversalTime());
            return s.Substring(1, s.Length - 2);
        }
    }
}
