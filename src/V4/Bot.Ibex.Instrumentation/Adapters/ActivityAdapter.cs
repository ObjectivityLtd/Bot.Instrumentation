namespace Bot.Ibex.Instrumentation.V4.Adapters
{
    using System;
    using Bot.Ibex.Instrumentation.Common.Extensions;
    using Bot.Ibex.Instrumentation.Common.Telemetry;
    using Common.Models;

    public class ActivityAdapter : IActivity
    {
        private readonly Microsoft.Bot.Schema.IActivity activity;

        public ActivityAdapter(Microsoft.Bot.Schema.IActivity activity)
        {
            this.activity = activity ?? throw new ArgumentNullException(nameof(activity));
        }

        public string TimeStampIso8601
        {
            get
            {
                if (this.activity.Timestamp != null)
                {
                    return this.activity.Timestamp.Value.AsIso8601();
                }
                else
                {
                    return null;
                }
            }
        }

        public string Type => this.activity.Type;

        public string ChannelId => this.activity.ChannelId;

        public string ReplyToId => this.activity.ReplyToId;

        public MessageActivity MessageActivity
        {
            get
            {
                if (this.activity.AsMessageActivity() != null)
                {
                    var messageActivity = new MessageActivity();
                    messageActivity.Text = this.activity.AsMessageActivity().Text;
                    messageActivity.Id = this.activity.AsMessageActivity().Id;
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
                    var channelAccount = new ChannelAccount();
                    channelAccount.Name = this.activity.From.Name;
                    channelAccount.Id = this.activity.From.Id;
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
