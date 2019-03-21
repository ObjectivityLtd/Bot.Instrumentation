namespace Bot.Ibex.Instrumentation.Dialogs
{
    using Autofac;
    using Bot.Ibex.Instrumentation.Instumentation;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Luis;
    using Microsoft.Bot.Builder.Luis.Models;
    using Microsoft.Bot.Connector;
    using System;
    using System.Threading.Tasks;

    [Serializable]
    public class InstrumentedLuisDialog<TResult> : LuisDialog<TResult>
    {
        public InstrumentedLuisDialog(LuisModelAttribute luisModel) : base(new LuisService(luisModel))
        {
        }

        public InstrumentedLuisDialog(string luisModelId, string luisSubscriptionKey) : base(new LuisService(new LuisModelAttribute(luisModelId, luisSubscriptionKey)))
        {
        }

        public InstrumentedLuisDialog(string luisModelId, string luisSubscriptionKey, string domain, double threshold = 0) : base(new LuisService(new LuisModelAttribute(luisModelId, luisSubscriptionKey, LuisApiVersion.V2, domain)))
        {
        }

        protected override Task DispatchToIntentHandler(IDialogContext context, IAwaitable<IMessageActivity> item, IntentRecommendation bestIntent, LuisResult result)
        {
            using (var scope = Conversation.Container.BeginLifetimeScope())
            {
                var service = scope.Resolve<IBotFrameworkInstrumentation>();
                service.TrackLuisIntent(context.Activity, result);
            }
            return base.DispatchToIntentHandler(context, item, bestIntent, result);
        }
    }
}
