# Bot.Instrumentation

[![Build Status](https://ci.appveyor.com/api/projects/status/github/ObjectivityLtd/Bot.Instrumentation?branch=master&svg=true)](https://ci.appveyor.com/project/ObjectivityAdminsTeam/bot-instrumentation) [![Tests Status](https://img.shields.io/appveyor/tests/ObjectivityAdminsTeam/bot-instrumentation/master.svg)](https://ci.appveyor.com/project/ObjectivityAdminsTeam/bot-instrumentation) [![codecov](https://codecov.io/gh/ObjectivityLtd/Bot.Instrumentation/branch/master/graph/badge.svg)](https://codecov.io/gh/ObjectivityLtd/Bot.Instrumentation)   [![nuget](https://img.shields.io/nuget/v/Bot.Instrumentation.V3.svg) ![Downloads](https://img.shields.io/nuget/dt/Bot.Instrumentation.V3.svg)](https://www.nuget.org/packages/Bot.Instrumentation.V3/) [![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://opensource.org/licenses/MIT)

Simplifies adding custom analytics for bots built with [Microsoft Bot Framework V3](https://dev.botframework.com) to leverage it with [Ibex Dashboard](https://github.com/Azure/ibex-dashboard).

## Getting Started

### Application insigths

Instrumentation stores bot telemetry data in [Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview) thus it is required to obtain the _Instrumentation Key_ which identifies the resource the telemetry data is associated with.

[Create an Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/create-new-resource) resource if you haven't already done that for your bot.

### ActivityInstrumentation

This instrumentation provides basic bot telemetry data and is extreamy powerfull in conjunction with `DialogActivityLogger` which logs your dialogs activities.

#### Example

##### Setup

Add the following code to your application start.

It is being assument that [Autofac](https://autofac.org/) is being used as IoC container.

```csharp
TelemetryConfiguration.Active.InstrumentationKey = "<INSTRUMENTATION_KEY>"
var telemetry = new TelemetryClient(TelemetryConfiguration.Active);
var settings = new InstrumentationSettings();

builder.RegisterInstance(new ActivityInstrumentation(telemetry, settings))
       .Keyed<IIntentInstrumentation>(FiberModule.Key_DoNotSerialize)
       .As<IActivityInstrumentation>();

builder.RegisterType<DialogActivityLogger>()
       .AsImplementedInterfaces()
       .InstancePerDependency();
```

* `<INSTRUMENTATION_KEY>` is an instrumentation key of Application Insights to be obtained once it is configured in Azure.

## Additional instrumentations

### QnAInstrumentation

Provides QnA Maker instrumentation.

#### Example

##### Setup

```csharp
TelemetryConfiguration.Active.InstrumentationKey = "<INSTRUMENTATION_KEY>"
var telemetry = new TelemetryClient(TelemetryConfiguration.Active);
var settings = new InstrumentationSettings();

builder.RegisterInstance(new QnAInstrumentation(telemetry, settings))
    .Keyed<IQnAInstrumentation>(FiberModule.Key_DoNotSerialize)
    .As<IQnAInstrumentation>();
```

* `<INSTRUMENTATION_KEY>` is an instrumentation key of Application Insights to be obtained once it is configured in Azure.

##### Usage

```csharp
private async Task HandleQnAMakerResults(
    QnAMakerResults results,
    IDialogContext context,
    IQnAInstrumentation instrumentation)
{
    if (results != null && results.Answers.Any())
    {
        instrumentation.TrackEvent(context.Activity, results);

        var topScore = results.Answers
                              .OrderByDescending(x => x.Score)
                              .First();
        await context.PostAsync(topScore.Answer);
    }
}
```

### IntentInstrumentation

Provides LUIS Intent instrumentation.

#### Example

##### Setup

```csharp
TelemetryConfiguration.Active.InstrumentationKey = "<INSTRUMENTATION_KEY>"
var telemetry = new TelemetryClient(TelemetryConfiguration.Active);
var settings = new InstrumentationSettings();

builder.RegisterInstance(IntentInstrumentation(telemetry, settings))
    .Keyed<IIntentInstrumentation>(FiberModule.Key_DoNotSerialize)
    .As<IIntentInstrumentation>();
```

* `<INSTRUMENTATION_KEY>` is an instrumentation key of Application Insights to be obtained once it is configured in Azure.

##### Usage

```csharp
private async Task HandleLuisResult(
    LuisResult result,
    IDialogContext context,
    IIntentInstrumentation instrumentation)
{
    if (result != null)
    {
        instrumentation.TrackIntent(context.Activity, result);
    }
}
```

### SentimentInstrumentation

Provides sentiment instrumentation.

#### Example

##### Setup

```csharp
TelemetryConfiguration.Active.InstrumentationKey = "<INSTRUMENTATION_KEY>"
var telemetry = new TelemetryClient(TelemetryConfiguration.Active);
var settings = new InstrumentationSettings();
var sentimentSettings = new SentimentClientSettings
{
    ApiSubscriptionKey = "<TEXT_ANALYTICS_SUBSCRIPTION_KEY>",
    Endpoint = "<COGNITIVE_SERVICES_ENDPOINT_URI>"
};
var sentiment = new SentimentClient(sentimentSettings);

builder.RegisterInstance(new SentimentInstrumentation(settings, telemetry, sentiment))
    .Keyed<ISentimentInstrumentation>(FiberModule.Key_DoNotSerialize)
    .As<ISentimentInstrumentation>();
```

* `<INSTRUMENTATION_KEY>` is an instrumentation key of Application Insights to be obtained once it is configured in Azure.
* `<TEXT_ANALYTICS_SUBSCRIPTION_KEY>` is a subscription key of the Text Analytics to be obtained once it is configured in Azure.
* `<COGNITIVE_SERVICES_ENDPOINT_URI>` is a supported endpoint of the Cognitive Services (protocol and hostname, for example: <https://westus.api.cognitive.microsoft.com>)

##### Usage

```csharp
private async Task HandleMessageSentiment(
    IDialogContext context,
    ISentimentInstrumentation instrumentation)
{
    await instrumentation.TrackMessageSentiment(context.Activity);
}
```

### CustomInstrumentation

Provides custom event instrumentation.

#### Example

##### Setup

```csharp
TelemetryConfiguration.Active.InstrumentationKey = "<INSTRUMENTATION_KEY>"
var telemetry = new TelemetryClient(TelemetryConfiguration.Active);
var settings = new InstrumentationSettings();

builder.RegisterInstance(new CustomInstrumentation(telemetry, settings))
    .Keyed<ICustomInstrumentation>(FiberModule.Key_DoNotSerialize)
    .As<ICustomInstrumentation>();
```

* `<INSTRUMENTATION_KEY>` is an instrumentation key of Application Insights to be obtained once it is configured in Azure.

##### Usage

```csharp
private void TrackConversationRating(
    IActivity activity,
    string rating,
    ICustomInstrumentation instrumentation)
{
    var properties = new Dictionary<string, string> { { "score", rating } };
    instrumentation.TrackCustomEvent(activity, "ConversationRating", properties);
}
```
