namespace Bot.Ibex.Instrumentation.V4.Adapters
{
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
    }
}
