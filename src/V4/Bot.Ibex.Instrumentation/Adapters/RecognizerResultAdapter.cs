namespace Bot.Ibex.Instrumentation.V4.Adapters
{
    using System;
    using System.Globalization;
    using Bot.Ibex.Instrumentation.Common.Models;
    using Microsoft.Bot.Builder;
    using Newtonsoft.Json;

    public class RecognizerResultAdapter
    {
        private readonly RecognizerResult result;

        public RecognizerResultAdapter(RecognizerResult result)
        {
            this.result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public RecognizedIntentResult ConvertRecognizerResultToRecognizedIntentResult()
        {
            var intentResult = new RecognizedIntentResult();
            var topScoringIntent = this.result.GetTopScoringIntent();

            intentResult.Intent = topScoringIntent.intent;
            intentResult.Score = topScoringIntent.score.ToString(CultureInfo.InvariantCulture);
            intentResult.Entities = this.result.Entities.ToString(Formatting.None);

            return intentResult;
        }
    }
}
