﻿namespace Bot.Instrumentation.V4.Adapters
{
    using System;
    using System.Globalization;
    using Bot.Instrumentation.Common.Adapters;
    using Bot.Instrumentation.Common.Models;
    using Microsoft.Bot.Builder;
    using Newtonsoft.Json;

    public class RecognizerResultAdapter : ILuisResultAdapter
    {
        private readonly RecognizerResult result;

        public RecognizerResultAdapter(RecognizerResult result)
        {
            this.result = result ?? throw new ArgumentNullException(nameof(result));
        }

        public IntentResult IntentResult
        {
            get
            {
                var topScoringIntent = this.result.GetTopScoringIntent();
                return new IntentResult
                {
                    Intent = topScoringIntent.intent,
                    Score = topScoringIntent.score.ToString(CultureInfo.InvariantCulture),
                    Entities = this.result.Entities.ToString(Formatting.None),
                };
            }
        }
    }
}
