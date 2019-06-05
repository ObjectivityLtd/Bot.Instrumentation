# Bot.Ibex.Instrumentation

[![Build Status](https://ci.appveyor.com/api/projects/status/github/ObjectivityLtd/Bot.Ibex.Instrumentation?branch=master&svg=true)](https://ci.appveyor.com/project/ObjectivityAdminsTeam/bot-ibex-instrumentation) [![Tests Status](https://img.shields.io/appveyor/tests/ObjectivityAdminsTeam/bot-ibex-instrumentation/master.svg)](https://ci.appveyor.com/project/ObjectivityAdminsTeam/bot-ibex-instrumentation) [![codecov](https://codecov.io/gh/ObjectivityLtd/Bot.Ibex.Instrumentation/branch/master/graph/badge.svg)](https://codecov.io/gh/ObjectivityLtd/Bot.Ibex.Instrumentation)   [![nuget](https://img.shields.io/nuget/v/Bot.Ibex.Instrumentation.V3.svg) ![Downloads](https://img.shields.io/nuget/dt/Bot.Ibex.Instrumentation.V3.svg)](https://www.nuget.org/packages/Bot.Ibex.Instrumentation.V3/) [![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://opensource.org/licenses/MIT)

Simplifies adding custom analytics for bots built with [Microsoft Bot Framework V3](https://dev.botframework.com) to leverage it with [Ibex Dashboard](https://github.com/Azure/ibex-dashboard).

### QnAInstrumentation

Provides QnA Maker instrumentation.

#### Example

##### Setup

```csharp
var telemetryClient = new TelemetryClient();
var instrumentationSettings = new InstrumentationSettings();

builder.Register(c => new QnAInstrumentation(telemetryClient, instrumentationSettings))
	.Keyed<IQnAInstrumentation>(FiberModule.Key_DoNotSerialize)
	.As<IQnAInstrumentation>();
```

##### Usage

```csharp
private async Task DispatchToQnAMakerAsync(
    QnAMaker qnaMaker,
    ITurnContext turnContext,
    IQnAInstrumentation instrumentation,
    CancellationToken cancellationToken = default(CancellationToken))
{
    if (!string.IsNullOrEmpty(turnContext.Activity.Text))
    {
        var results = await qnaMaker.GetAnswersAsync(turnContext)
            .ConfigureAwait(false);

        if (results.Any())
        {
            var result = results.First();
            instrumentation.TrackEvent(turnContext.Activity, result);
        }
    }
}
```

### IntentInstrumentation

Provides LUIS Intent instrumentation.

#### Example

##### Setup

```csharp
var telemetryClient = new TelemetryClient();
var instrumentationSettings = new InstrumentationSettings();

builder.Register(c => new IntentInstrumentation(telemetryClient, instrumentationSettings))
	.Keyed<IIntentInstrumentation>(FiberModule.Key_DoNotSerialize)
	.As<IIntentInstrumentation>();
```

##### Usage

```csharp
private async Task DispatchToQnAMakerAsync(
    LuisResult result,
    IDialogContext activity,
    IIntentInstrumentation intentInstrumentation,
    CancellationToken cancellationToken = default(CancellationToken))
{
	if (results.Any())
	{
		intentInstrumentation.TrackIntent(activity.Activity, result);
	}
}
```

### SentimentInstrumentation

Provides sentiment instrumentation.

#### Example

##### Setup

```csharp
var sentimentSettings = new SentimentClientSettings
{
	ApiSubscriptionKey = "<TEXT_ANALYTICS_SUBSCRIPTION_KEY>",
	Endpoint = "<COGNITIVE_SERVICES_ENDPOINT_URI>"
};
var sentimentClient = new SentimentClient(sentimentSettings);
var instrumentationSettings = new InstrumentationSettings();
var telemetryClient = new TelemetryClient();

builder.Register(c => new SentimentInstrumentation(instrumentationSettings, telemetryClient, sentimentClient))
	.Keyed<ISentimentInstrumentation>(FiberModule.Key_DoNotSerialize)
	.As<ISentimentInstrumentation>();
```

* `<TEXT_ANALYTICS_SUBSCRIPTION_KEY>` is a subscription key of the Text Analytics to be obtained once it is configured in Azure.
* `<COGNITIVE_SERVICES_ENDPOINT_URI>` is a supported endpoint of the Cognitive Services (protocol and hostname, for example: https://westus.api.cognitive.microsoft.com)
* `<INSTRUMENTATION_KEY>` is an instrumentation key of Application Insights to be obtained once it is configured in Azure.

##### Usage

```csharp
private async Task HandleMessageSentiment(
    IDialogContext turnContext,
    ISentimentInstrumentation instrumentation)
{
    if (turnContext.Activity.IsIncomingMessage())
    {
        await this.sentimentInstrumentation.TrackMessageSentiment(context.Activity)
            .ConfigureAwait(false);
    }
}
```

### CustomInstrumentation

Provides custom event instrumentation.

#### Example

##### Setup

```csharp
var telemetryClient = new TelemetryClient();
var instrumentationSettings = new InstrumentationSettings();

builder.Register(c => new CustomInstrumentation(telemetryClient, instrumentationSettings))
	.Keyed<ICustomInstrumentation>(FiberModule.Key_DoNotSerialize)
	.As<ICustomInstrumentation>();
```

##### Usage

```csharp
private void TrackConversationRating(
    IActivity activity,
    string rating,
    ICustomInstrumentation instrumentation)
{
    instrumentation.TrackCustomEvent(activity);
}
```

### ActivityInstrumentation

Provides basic instrumentation.

#### Example

##### Setup

```csharp
var telemetryClient = new TelemetryClient();
var instrumentationSettings = new InstrumentationSettings();

builder.Register(c => new ActivityInstrumentation(telemetryClient, instrumentationSettings)).As<IActivityInstrumentation>();

builder.RegisterType<DialogActivityLogger>().AsImplementedInterfaces().InstancePerDependency();
```

##### Usage

In Application_Start() add:

```csharp
var appInsightsKey = WebConfigurationManager.AppSettings["<INSTRUMENTATION_KEY>"];
if (!string.IsNullOrEmpty(appInsightsKey))
{
	TelemetryConfiguration.Active.InstrumentationKey = appInsightsKey;
}
else
{
	throw new ConfigurationErrorsException("Application Insights instrumentation key can not be null");
}
```

* `<INSTRUMENTATION_KEY>` is an instrumentation key of Application Insights to be obtained once it is configured in Azure.