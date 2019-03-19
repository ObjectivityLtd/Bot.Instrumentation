namespace Bot.Ibex.Instrumentation.Telemetry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Bot.Ibex.Instrumentation.Extensions;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Bot.Schema;

    public class EventTelemetryBuilder
    {
        private readonly IActivity activity;
        private readonly InstrumentationSettings settings;
        private readonly IEnumerable<KeyValuePair<string, string>> properties;

        public EventTelemetryBuilder(IActivity activity, InstrumentationSettings settings, IEnumerable<KeyValuePair<string, string>> properties = null)
        {
            this.activity = activity ?? throw new ArgumentNullException(nameof(activity));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));
            this.properties = properties ?? Enumerable.Empty<KeyValuePair<string, string>>();
        }

        public EventTelemetry Build()
        {
            var et = new EventTelemetry();
            if (this.activity.Timestamp != null)
            {
                et.Properties.Add(BotConstants.TimestampProperty, this.activity.Timestamp.Value.AsIso8601());
            }

            et.Properties.Add(BotConstants.TypeProperty, this.activity.Type);
            et.Properties.Add(BotConstants.ChannelProperty, this.activity.ChannelId);

            switch (this.activity.Type)
            {
                case ActivityTypes.Message:
                    var messageActivity = this.activity.AsMessageActivity();
                    if (this.activity.ReplyToId == null)
                    {
                        et.Name = EventTypes.MessageReceived;
                        et.Properties.Add(BotConstants.UserIdProperty, this.activity.From.Id);
                        if (!this.settings.OmitUsernameFromTelemetry)
                        {
                            et.Properties.Add(BotConstants.UserNameProperty, this.activity.From.Name);
                        }
                    }
                    else
                    {
                        et.Name = EventTypes.MessageSent;
                    }

                    et.Properties.Add(BotConstants.TextProperty, messageActivity.Text);
                    et.Properties.Add(BotConstants.ConversationIdProperty, messageActivity.Conversation.Id);
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

            foreach (var property in this.properties)
            {
                et.Properties.Add(property);
            }

            return et;
        }
    }
}
