namespace Bot.Ibex.Instrumentation.V4.Adapters
{
    using System;
    using System.Globalization;
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Models;
    using Microsoft.Bot.Builder;
    using Newtonsoft.Json;

    public class RecognizerResultAdapter : ILuisResultAdapter
    {
        private readonly RecognizerResult result;

        public RecognizerResultAdapter(RecognizerResult result)
        {
            this.result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public IntentResult IntentResult => this.ConvertRecognizerResultIntentResult();

        public IntentResult ConvertRecognizerResultIntentResult()
        {
            var topScoringIntent = this.result.GetTopScoringIntent();
            var intentResult = new IntentResult
            {
                Intent = topScoringIntent.intent,
                Score = topScoringIntent.score.ToString(CultureInfo.InvariantCulture),
                Entities = this.result.Entities.ToString(Formatting.None),
            };

            return intentResult;
        }
    }
}
