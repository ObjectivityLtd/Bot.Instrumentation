namespace Bot.Ibex.Instrumentation.V4.Adapters
{
    using System;
    using Bot.Ibex.Instrumentation.Common.Extensions;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Common.Models;
    using Microsoft.Bot.Builder;

    public class TurnContextAdapter : IActivity
    {
        private readonly ITurnContext activity;

        public TurnContextAdapter(ITurnContext activity)
        {
            this.activity = activity ?? throw new ArgumentNullException(nameof(activity));
        }

        public string TimeStampIso8601 => this.activity.Activity.Timestamp?.AsIso8601();

        public string Type => this.activity.Activity.Type;

        public string ChannelId => this.activity.Activity.ChannelId;

        public string ReplyToId => this.activity.Activity.ReplyToId;

        public MessageActivity MessageActivity
        {
            get
            {
                if (this.activity.Activity.Text != null)
                {
                    var messageActivity = new MessageActivity
                    {
                        Text = this.activity.Activity.Text,
                        Id = this.activity.Activity.Id
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
                if (this.activity.Activity != null)
                {
                    var channelAccount = new ChannelAccount
                    {
                        Name = this.activity.Activity.Name,
                        Id = this.activity.Activity.Id
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
