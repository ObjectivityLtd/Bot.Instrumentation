namespace Bot.Ibex.Instrumentation.V4.Adapters
{
    using System.Globalization;
    using Microsoft.Bot.Builder;
    using Newtonsoft.Json;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Models;

    public class RecognizerResultAdapter
    {
        private readonly RecognizerResult result;

        public RecognizerResultAdapter(RecognizerResult result)
        {
            this.result = result;
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
