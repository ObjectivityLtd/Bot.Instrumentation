namespace Bot.Ibex.Instrumentation.Instumentation
{
    using Autofac;
    using Bot.Ibex.Instrumentation.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.History;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Extensions;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    public class BotFrameworkInstrumentation : IBotFrameworkInstrumentation
    {
        private readonly List<TelemetryClient> telemetryClients;
        private readonly InstrumentationSettings settings;

        public BotFrameworkInstrumentation(InstrumentationSettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            if (settings.InstrumentationKeys == null || settings.InstrumentationKeys.Count == 0)
            {
                throw new ArgumentException("Settings must contain at least one instrumentation key");
            }
            this.settings = settings;

            //init clients
            telemetryClients = new List<TelemetryClient>();
            this.settings.InstrumentationKeys.ForEach(key =>
            {
                //_telemetryClients.Add(new TelemetryClient(new TelemetryConfiguration(key)));
                telemetryClients.Add(new TelemetryClient());
            });

            // Register activity logger via autofac DI.
            Conversation.UpdateContainer(builder =>
            {
                builder.RegisterType<DialogActivityLogger>().As<IActivityLogger>().InstancePerLifetimeScope();
                builder.RegisterInstance<IBotFrameworkInstrumentation>(this).As<IBotFrameworkInstrumentation>()
                    .SingleInstance();
            });
        }

        public async Task TrackActivity(IActivity activity, IBotData botData = null,
            IDictionary<string, string> customProperties = null)
        {
            var et = BuildEventTelemetry(activity, customProperties);

            telemetryClients.ForEach(c => c.TrackEvent(et));

            // Track sentiment only for incoming messages. 
            if (et.Name == EventTypes.MessageReceived)
            {
                await TrackMessageSentiment(activity, customProperties);
            }
        }

        public void TrackLuisIntent(IActivity activity, LuisResult result)
        {
            if (result?.TopScoringIntent == null)
            {
                return;
            }
            var properties = new Dictionary<string, string>
            {
                {"intent", result.TopScoringIntent.Intent},
                {"score", result.TopScoringIntent.Score.ToString()},
                {"entities", JsonConvert.SerializeObject(result.Entities)}
            };

            var eventTelemetry = BuildEventTelemetry(activity, properties);
            eventTelemetry.Name = EventTypes.Intent;
            telemetryClients.ForEach(c => c.TrackEvent(eventTelemetry));
        }

        public void TrackQnaEvent(IActivity activity, string userQuery, string kbQuestion, string kbAnswer, double score)
        {
            var properties = new Dictionary<string, string>
            {
                {"userQuery", userQuery},
                {"kbQuestion", kbQuestion},
                {"kbAnswer", kbAnswer},
                {"score", score.ToString(CultureInfo.InvariantCulture)}
            };

            var eventTelemetry = BuildEventTelemetry(activity, properties);
            eventTelemetry.Name = EventTypes.QnaEvent;
            telemetryClients.ForEach(c => c.TrackEvent(eventTelemetry));
        }

        public void TrackCustomEvent(IActivity activity, string eventName = EventTypes.CustomEvent,
            IDictionary<string, string> customEventProperties = null)
        {
            var eventTelemetry = BuildEventTelemetry(activity, customEventProperties);
            eventTelemetry.Name = string.IsNullOrWhiteSpace(eventName) ? EventTypes.CustomEvent : eventName;
            telemetryClients.ForEach(c => c.TrackEvent(eventTelemetry));
        }

        private async Task TrackMessageSentiment(IActivity activity, IDictionary<string, string> customProperties = null)
        {
            if (settings.SentimentManager == null)
            {
                return;
            }

            var properties = await settings.SentimentManager.GetSentimentProperties(activity.AsMessageActivity().Text);
            if (properties != null)
            {
                //if there are custom properties, also add them.
                if (customProperties != null)
                {
                    foreach (var kvp in customProperties)
                    {
                        if (!properties.ContainsKey(kvp.Key))
                        {
                            properties.Add(kvp.Key, kvp.Value);
                        }
                    }
                }

                var et = BuildEventTelemetry(activity, properties);
                et.Name = EventTypes.MessageSentiment;
                telemetryClients.ForEach(c => c.TrackEvent(et));
            }
        }

        public void TrackGoalTriggeredEvent(IActivity activity, string goalName,
            IDictionary<string, string> goalTriggeredEventProperties = null)
        {
            if (goalTriggeredEventProperties == null)
                goalTriggeredEventProperties = new Dictionary<string, string>();

            goalTriggeredEventProperties.Add("GoalName", goalName);

            var eventTelemetry = BuildEventTelemetry(activity, goalTriggeredEventProperties);
            eventTelemetry.Name = EventTypes.GoalTriggeredEvent;
            telemetryClients.ForEach(c => c.TrackEvent(eventTelemetry));
        }

        /// <summary>
        /// Helper method to create an EventTelemetry instance and populate common properties depending on the message type.
        /// </summary>
        private EventTelemetry BuildEventTelemetry(IActivity activity, IDictionary<string, string> properties = null,
            IDictionary<string, double> metrics = null)
        {
            var et = new EventTelemetry();
            if (activity.Timestamp != null)
                et.Properties.Add("timestamp", DateTimeExtensions.AsIso8601(activity.Timestamp.Value));

            et.Properties.Add("type", activity.Type);
            et.Properties.Add("channel", activity.ChannelId);

            switch (activity.Type)
            {
                case ActivityTypes.Message:
                    var messageActivity = activity.AsMessageActivity();
                    if (activity.ReplyToId == null)
                    {
                        et.Name = EventTypes.MessageReceived;
                        et.Properties.Add("userId", activity.From.Id);
                        if (!settings.OmitUsernameFromTelemetry)
                        {
                            et.Properties.Add("userName", activity.From.Name);
                        }
                    }
                    else
                    {
                        et.Name = EventTypes.MessageSent;
                    }
                    et.Properties.Add("text", messageActivity.Text);
                    et.Properties.Add("conversationId", messageActivity.Conversation.Id);
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

            // Add any other properties received.
            if (properties != null)
            {
                foreach (var property in properties)
                {
                    et.Properties.Add(property);
                }
            }

            // Add any other metrics received.
            if (metrics != null)
            {
                foreach (var metric in metrics)
                {
                    et.Metrics.Add(metric);
                }
            }

            return et;
        }
    }
}
