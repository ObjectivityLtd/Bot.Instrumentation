namespace Bot.Ibex.Instrumentation.V3.Adapters
{
    using System;
    using Newtonsoft.Json;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;

    public class ActivityAdapter : IActivity
    {
        private readonly Microsoft.Bot.Connector.IActivity activity;

        public ActivityAdapter(Microsoft.Bot.Connector.IActivity activity)
        {
            this.activity = activity ?? throw new ArgumentNullException(nameof(activity));
        }

        public string TimeStampIso8601
        {
            get
            {
                var s = JsonConvert.SerializeObject(this.activity.Timestamp?.ToUniversalTime());
                return s.Substring(1, s.Length - 2);
            }
        }

        public string Type => this.activity.Type;

        public string ChannelId => this.activity.ChannelId;

        public string ReplyToId => this.activity.ReplyToId;

        public MessageActivity MessageActivity
        {
            get
            {
                var messageActivity = new MessageActivity();
                messageActivity.Text = this.activity.AsMessageActivity().Text;
                messageActivity.Id = this.activity.AsMessageActivity().Id;
                return messageActivity;
            }
        }

        public ChannelAccount ChannelAccount
        {
            get
            {
                var channelAccount = new ChannelAccount();
                channelAccount.Name = this.activity.From.Name;
                channelAccount.Id = this.activity.From.Id;
                return channelAccount;
            }
        }
    }
}
