namespace Bot.Ibex.Instrumentation.V4.Adapters
{
    using System.Globalization;
    using Microsoft.Bot.Builder;
    using Newtonsoft.Json;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Telemetry;

    public class ActivityExtensionsAdapter : IActivity
    {
        private readonly ITurnContext activity;

        public ActivityExtensionsAdapter(ITurnContext activity)
        {
            this.activity = activity;
        }

        public string TimeStampIso8601
        {
            get
            {
                var s = JsonConvert.SerializeObject(this.activity.Activity.Timestamp.Value.ToUniversalTime());
                return s.Substring(1, s.Length - 2);
            }
        }

        public string Type => this.activity.Activity.Type;

        public string ChannelId => this.activity.Activity.ChannelId;

        public string ReplyToId => this.activity.Activity.ReplyToId;

        public MessageActivity MessageActivity
        {
            get
            {
                var messageActivity = new MessageActivity();
                messageActivity.Text = this.activity.Activity.Text;
                messageActivity.Id = this.activity.Activity.Id;
                return messageActivity;
            }
        }

        public ChannelAccount ChannelAccount
        {
            get
            {
                var channelAccount = new ChannelAccount();
                channelAccount.Name = this.activity.Activity.Name;
                channelAccount.Id = this.activity.Activity.Id;
                return channelAccount;
            }
        }

        public Activity Activity
        {
            get
            {
                var activity = new Activity();
                activity.MessageActivity.Text = this.activity.Activity.AsMessageActivity().Text;
                activity.MessageActivity.Id = this.activity.Activity.AsMessageActivity().Id;
                activity.ChannelAccount.Id = this.activity.Activity.From.Id;
                activity.ChannelAccount.Name = this.activity.Activity.From.Name;
                activity.ChannelId = this.activity.Activity.ChannelId;
                activity.ReplyToId = this.activity.Activity.ReplyToId;
                activity.TimeStampIso8601 = this.activity.Activity.Timestamp.ToString();
                activity.Type = this.activity.Activity.Type;
                return activity;
            }
        }
    }
}
