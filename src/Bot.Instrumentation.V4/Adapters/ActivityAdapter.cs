namespace Bot.Instrumentation.V4.Adapters
{
    using System;
    using Bot.Instrumentation.Common.Adapters;
    using Bot.Instrumentation.Common.Extensions;
    using Bot.Instrumentation.Common.Models;

    public class ActivityAdapter : IActivityAdapter
    {
        private readonly Microsoft.Bot.Schema.IActivity activity;

        public ActivityAdapter(Microsoft.Bot.Schema.IActivity activity)
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
                var messageActivity = this.activity.AsMessageActivity();
                if (messageActivity != null)
                {
                    return new MessageActivity
                    {
                        Text = messageActivity.Text,
                        Id = messageActivity.Id,
                    };
                }

                return null;
            }
        }

        public ChannelAccount ChannelAccount
        {
            get
            {
                var from = this.activity.From;
                if (from != null)
                {
                    return new ChannelAccount
                    {
                        Name = from.Name,
                        Id = from.Id,
                    };
                }

                return null;
            }
        }
    }
}
