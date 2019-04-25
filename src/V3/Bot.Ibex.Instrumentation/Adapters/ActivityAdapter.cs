namespace Bot.Ibex.Instrumentation.V3.Adapters
{
    using System;
    using Bot.Ibex.Instrumentation.Common.Extensions;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Common.Models;

    public class ActivityAdapter : IActivityAdapter
    {
        private readonly Microsoft.Bot.Connector.IActivity activity;

        public ActivityAdapter(Microsoft.Bot.Connector.IActivity activity)
        {
            this.activity = activity ?? throw new ArgumentNullException(nameof(activity));
        }

        public string TimeStampIso8601 => this.activity.Timestamp?.AsIso8601();

        public string Type => this.activity.Type;

        public string ChannelId => this.activity.ChannelId;

        public string ReplyToId => this.activity.ReplyToId;

        public MessageActivity MessageActivity
        {
            get
            {
                if (this.activity.AsMessageActivity() != null)
                {
                    var messageActivity = new MessageActivity
                    {
                        Text = this.activity.AsMessageActivity().Text,
                        Id = this.activity.AsMessageActivity().Id
                    };

                    return messageActivity;
                }
                else
                {
                    return null;
                }
            }
        }

        public ChannelAccount ChannelAccount
        {
            get
            {
                if (this.activity.From != null)
                {
                    var channelAccount = new ChannelAccount
                    {
                        Name = this.activity.From.Name,
                        Id = this.activity.From.Id
                    };

                    return channelAccount;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
