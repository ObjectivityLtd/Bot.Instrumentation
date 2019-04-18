namespace Bot.Ibex.Instrumentation.V3.Adapters
{
    using System;
    using Microsoft.Bot.Builder.Luis.Models;
    using Newtonsoft.Json;
    using Objectivity.Bot.Ibex.Instrumentation.Common.Models;

    public class LuisResultAdapter
    {
        private readonly LuisResult result;

        public LuisResultAdapter(LuisResult result)
        {
            this.result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public RecognizedIntentResult ConvertLuisResultToRecognizedIntentResult()
        {
            var intentResult = new RecognizedIntentResult();

            intentResult.Intent = this.result.TopScoringIntent.Intent;
            intentResult.Score = this.result.TopScoringIntent.Score.ToString();
            intentResult.Entities = JsonConvert.SerializeObject(this.result.Entities);

            return intentResult;
        }
    }
}
