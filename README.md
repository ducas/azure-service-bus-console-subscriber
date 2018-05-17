# Azure Service Bus Console Subscriber
Azure Service Bus subscriber that prints all messages to the console.

## Getting Started
Update the value of *ServiceBus* in appsettings.json with a valid connection string that includes the EntityPath, and the *SubscriptionName* to the name of a subscription you have created. Then just execute `dotnet run -p ServiceBusConsoleSubscriber/ServiceBusConsoleSubscriber.csproj`.

## What it does
This app simply subscribes to a Service Bus of your choosing and outputs all messages to the console.

## Why?!
I was looking for a way to verify my publisher was sending correctly formatted messages to a Service Bus, so I needed to pump the messages out to a console...
