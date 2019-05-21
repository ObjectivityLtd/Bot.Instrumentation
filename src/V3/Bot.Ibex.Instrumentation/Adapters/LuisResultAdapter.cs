namespace Bot.Ibex.Instrumentation.V3.Adapters
{
    using System;
    using Bot.Ibex.Instrumentation.Common.Adapters;
    using Bot.Ibex.Instrumentation.Common.Models;
    using Microsoft.Bot.Builder.Luis.Models;
    using Newtonsoft.Json;

    public class LuisResultAdapter : ILuisResultAdapter
    {
        private readonly LuisResult result;

        public LuisResultAdapter(LuisResult result)
        {
            this.result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public IntentResult IntentResult => this.ConvertLuisesultToIntentResult();

        public IntentResult ConvertLuisesultToIntentResult()
        {
            var intentResult = new IntentResult
            {
                Intent = this.result.TopScoringIntent.Intent,
                Score = this.result.TopScoringIntent.Score.ToString(),
                Entities = JsonConvert.SerializeObject(this.result.Entities),
            };

            return intentResult;
        }
    }
}
