namespace Bot.Instrumentation.Common.Telemetry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bot.Instrumentation.Common.Adapters;
    using Bot.Instrumentation.Common.Settings;
    using Microsoft.ApplicationInsights.DataContracts;

    public class EventTelemetryBuilder
    {
        private readonly IActivityAdapter activity;
        private readonly InstrumentationSettings settings;
        private readonly IEnumerable<KeyValuePair<string, string>> additionalProperties;

        public EventTelemetryBuilder(IActivityAdapter activity, InstrumentationSettings settings, IEnumerable<KeyValuePair<string, string>> additionalProperties = null)
        {
            this.activity = activity ?? throw new ArgumentNullException(nameof(activity));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.additionalProperties = additionalProperties ?? Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public EventTelemetry Build()
        {
            var et = new EventTelemetry();
            var properties = new Dictionary<string, string>();

            if (this.activity.TimeStampIso8601 != null)
            {
                properties.Add(BotConstants.TimestampProperty, this.activity.TimeStampIso8601);
            }

            properties.Add(BotConstants.TypeProperty, this.activity.Type);
            properties.Add(BotConstants.ChannelProperty, this.activity.ChannelId);

            switch (this.activity.Type)
            {
                case ActivityTypes.Message:
                    if (this.activity.ReplyToId == null)
                    {
                        et.Name = EventTypes.MessageReceived;
                        properties.Add(BotConstants.UserIdProperty, this.activity.ChannelAccount.Id);
                        if (!this.settings.OmitUsernameFromTelemetry)
                        {
                            properties.Add(BotConstants.UserNameProperty, this.activity.ChannelAccount.Name);
                        }
                    }
                    else
                    {
                        et.Name = EventTypes.MessageSent;
                    }

                    properties.Add(BotConstants.TextProperty, this.activity.MessageActivity.Text);
                    properties.Add(BotConstants.ConversationIdProperty, this.activity.MessageActivity.Id);
                    break;
                case ActivityTypes.ConversationUpdate:
                    et.Name = EventTypes.ConversationUpdate;
                    break;
                case ActivityTypes.EndOfConversation:
                    et.Name = EventTypes.ConversationEnded;
                    break;
                default:
                    et.Name = EventTypes.OtherActivity;
                    break;
            }

            var eventProperties = this.additionalProperties.Concat(properties)
                .GroupBy(kv => kv.Key)
                .ToDictionary(g => g.Key, g => g.First().Value);

            foreach (var property in eventProperties)
            {
                et.Properties.Add(property);
            }

            return et;
        }
    }
}
