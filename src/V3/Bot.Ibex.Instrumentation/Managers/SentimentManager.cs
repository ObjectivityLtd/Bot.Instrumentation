namespace Bot.Ibex.Instrumentation.Managers
{
    using Bot.Ibex.Instrumentation.Communication;
    using Bot.Ibex.Instrumentation.Telemetry;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Threading.Tasks;

    public class SentimentManager
    {
        private readonly string textAnalyticsApiKey;
        private readonly string cognitiveServiceApiEndpoint;
        private readonly int textAnalyticsMinLength;
        private readonly IDictionary<string, string> httpHeaders;
        private readonly IHttpCommunication httpCommunication;

        private const string SentimentApiRoute = "text/analytics/v2.0/sentiment";
        private const string SubscriptionKey = "Ocp-Apim-Subscription-Key";

        public SentimentManager(string textAnalyiticsApiKey, string textAnalyticsMinLength,
            string cognitiveServiceApiEndpoint, IHttpCommunication httpCommunication = null)
        {
            textAnalyticsApiKey = textAnalyiticsApiKey;
            if (!string.IsNullOrWhiteSpace(textAnalyticsApiKey))
            {
                httpHeaders = new Dictionary<string, string> { { SubscriptionKey, textAnalyticsApiKey } };
            }

            this.cognitiveServiceApiEndpoint = cognitiveServiceApiEndpoint;

            if (!int.TryParse(textAnalyticsMinLength, out this.textAnalyticsMinLength))
            {
                this.textAnalyticsMinLength = 0;
            }

            this.httpCommunication = httpCommunication ?? new HttpCommunication();
        }

        /// <summary>
        /// Helper method to track the sentiment of incoming messages.
        /// </summary>
        public async Task<Dictionary<string, string>> GetSentimentProperties(string text)
        {
            if (string.IsNullOrWhiteSpace(textAnalyticsApiKey))
            {
                return null;
            }

            var numWords = text.Split(' ').Length;
            if (numWords >= textAnalyticsMinLength)
            {
                var score = await GetSentimentScore(text);
                if (score != null)
                {
                    return new Dictionary<string, string>
                    {
                        {"score", score.Value.ToString(CultureInfo.InvariantCulture)}
                    };
                }
            }

            return null;
        }

        private async Task<double?> GetSentimentScore(string message)
        {
            var docs = new List<DocumentInput>
            {
                new DocumentInput {Id = 1, Text = message}
            };
            var sentimentInput = new BatchInput { Documents = docs };
            var jsonSentimentInput = JsonConvert.SerializeObject(sentimentInput);
            var sentimentInfo = await GetSentiment(jsonSentimentInput);
            return sentimentInfo?.Documents[0].Score;
        }

        private async Task<BatchResult> GetSentiment(string jsonSentimentInput)
        {
            var data = Encoding.UTF8.GetBytes(jsonSentimentInput);

            var sentimentRawResponse = await httpCommunication.PostAsync(cognitiveServiceApiEndpoint,
                SentimentApiRoute, httpHeaders, data);

            return sentimentRawResponse == null
                ? null
                : JsonConvert.DeserializeObject<BatchResult>(sentimentRawResponse);
        }
    }
}
