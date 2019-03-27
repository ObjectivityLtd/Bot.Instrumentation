namespace Objectivity.Bot.Ibex.Instrumentation.Common.Settings
{
    using System;

    [Serializable]
    public class InstrumentationSettings
    {
        public bool OmitUsernameFromTelemetry { get; set; }
    }
}
