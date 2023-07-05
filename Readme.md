WebScale
========

[![Nuget](https://img.shields.io/nuget/v/WebScale?logo=nuget)](https://www.nuget.org/packages/WebScale/) [![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/Aldaviva/WebScale.net/dotnetpackage.yml?branch=master&logo=github)](https://github.com/Aldaviva/WebScale.net/actions/workflows/dotnetpackage.yml) [![Testspace](https://img.shields.io/testspace/tests/Aldaviva/Aldaviva:WebScale.net/master?passed_label=passing&failed_label=failing&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA4NTkgODYxIj48cGF0aCBkPSJtNTk4IDUxMy05NCA5NCAyOCAyNyA5NC05NC0yOC0yN3pNMzA2IDIyNmwtOTQgOTQgMjggMjggOTQtOTQtMjgtMjh6bS00NiAyODctMjcgMjcgOTQgOTQgMjctMjctOTQtOTR6bTI5My0yODctMjcgMjggOTQgOTQgMjctMjgtOTQtOTR6TTQzMiA4NjFjNDEuMzMgMCA3Ni44My0xNC42NyAxMDYuNS00NFM1ODMgNzUyIDU4MyA3MTBjMC00MS4zMy0xNC44My03Ni44My00NC41LTEwNi41UzQ3My4zMyA1NTkgNDMyIDU1OWMtNDIgMC03Ny42NyAxNC44My0xMDcgNDQuNXMtNDQgNjUuMTctNDQgMTA2LjVjMCA0MiAxNC42NyA3Ny42NyA0NCAxMDdzNjUgNDQgMTA3IDQ0em0wLTU1OWM0MS4zMyAwIDc2LjgzLTE0LjgzIDEwNi41LTQ0LjVTNTgzIDE5Mi4zMyA1ODMgMTUxYzAtNDItMTQuODMtNzcuNjctNDQuNS0xMDdTNDczLjMzIDAgNDMyIDBjLTQyIDAtNzcuNjcgMTQuNjctMTA3IDQ0cy00NCA2NS00NCAxMDdjMCA0MS4zMyAxNC42NyA3Ni44MyA0NCAxMDYuNVMzOTAgMzAyIDQzMiAzMDJ6bTI3NiAyODJjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjY3IDE0LjY3LTEwNiA0NHMtNDQgNjUtNDQgMTA3YzAgNDEuMzMgMTQuNjcgNzYuODMgNDQgMTA2LjVTNjY2LjY3IDU4NCA3MDggNTg0em0tNTU3IDBjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjgzIDE0LjY3LTEwNi41IDQ0UzAgMzkxIDAgNDMzYzAgNDEuMzMgMTQuODMgNzYuODMgNDQuNSAxMDYuNVMxMDkuNjcgNTg0IDE1MSA1ODR6IiBmaWxsPSIjZmZmIi8%2BPC9zdmc%2B)](https://aldaviva.testspace.com/spaces/210815) [![Coveralls](https://img.shields.io/coveralls/github/Aldaviva/WebScale.net?logo=coveralls)](https://coveralls.io/github/Aldaviva/WebScale.net?branch=master)

*Measure weight using a Stamps.com digital USB postage scale*

![Stamps.com 5-pound digital postage scale](https://raw.githubusercontent.com/Aldaviva/WebScale.net/master/.github/images/readme-header.jpg)

<!-- MarkdownTOC autolink="true" bracket="round" autoanchor="false" levels="1,2,3" bullets="1.,-,-,-" -->

1. [Quick Start](#quick-start)
1. [Prerequisites](#prerequisites)
1. [Installation](#installation)
1. [Usage](#usage)
    - [Connect to device](#connect-to-device)
    - [Read weight](#read-weight)
    - [Listen for weight change events](#listen-for-weight-change-events)
    - [Tare](#tare)
    - [Dispose](#dispose)
1. [Sample](#sample)
1. [See also](#see-also)

<!-- /MarkdownTOC -->

## Quick Start
```ps1
dotnet new console
dotnet add package WebScale
```
```cs
// Program.cs
using Aldaviva.WebScale;

using IWebScale webScale = new WebScale();

webScale.WeightChanged += (sender, weight) => Console.WriteLine($"{weight.OunceForce,4:N1} oz.");

// Keep console program running while waiting for WeightChanged events
CancellationTokenSource cancellationTokenSource = new();
Console.CancelKeyPress += (sender, eventArgs) => {
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};
Console.WriteLine("Press Ctrl+C to exit");
cancellationTokenSource.Token.WaitHandle.WaitOne();
```
```ps1
dotnet run
```

## Prerequisites

- Stamps.com Stainless Steel 5 lb. Digital Scale
    1. Sign up for free trial at [Stamps.com](https://registration.stamps.com/registration/).
    2. During signup, request a complimentary [5-pound digital postage scale](https://store.stamps.com/collections/hardware/products/5lb-scale). Pay $9.99 USD shipping and handling.
    3. Before the four-week trial is up, [cancel your account](https://www.stamps.com/postage-online/faqs/) and avoid the monthly fee.

    *I successfully performed these steps in [September 2014](https://twitter.com/Aldaviva/status/516771809962651648). Today, it may work differently, or not at all.*
- Any .NET runtime that supports [.NET Standard 2.0 or later](https://docs.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0#net-standard-versions):
    - [.NET 5.0 or later](https://dotnet.microsoft.com/en-us/download/dotnet)
    - [.NET Core 2.0 or later](https://dotnet.microsoft.com/en-us/download/dotnet)
    - [.NET Framework 4.6.1 or later](https://dotnet.microsoft.com/en-us/download/dotnet-framework)
- Operating systems:
    - Windows
    - MacOS
    - Linux (may need superuser permissions to access `/dev/hidraw0`)

## Installation

This package is available as [`WebScale` on NuGet Gallery](https://www.nuget.org/packages/WebScale).

```ps1
dotnet add package WebScale
```
```ps1
NuGet\Install-Package WebScale
```

## Usage

### Connect to device
Construct a new `WebScale` instance.

```cs
using Aldaviva.WebScale;

using IWebScale webScale = new WebScale();
```

This connects to the first Stamps.com Stainless Steel 5 lb. Digital Scale found connected to the computer.

If there is no suitable device connected, this will wait asynchronously and connect when one appears. If the device disconnects, this will wait for it to reconnect and then automatically reestablish a connection.

The `IsConnected` property shows whether or not the instance is currently connected to a scale. Updates to this property are indicated by the `IsConnectedChanged` event.

### Read weight

Get how much weight is currently on the scale. The precision is 0.1 ounces. Can be negative if the scale was tared with weight on it which was subsequently reduced.

```cs
Force weight = webScale.Weight;
```

The `Force` type is from the [UnitsNet](https://github.com/angularsen/UnitsNet) library. You can get specific units of force using properties on it like `OunceForce` or `KilogramsForce`.

This property automatically updates every 400 ± 15 milliseconds whenever the scale is connected.

### Listen for weight change events

Receive a notification when the weight on the scale changes. Not fired while taring.

```cs
webScale.WeightChanged += (sender, weight) => Console.WriteLine($"{weight.OunceForce,4:N1} oz.");
```

The event argument is the amount of weight currently on the scale.

If you need event callbacks to run on the UI thread so you can update a Windows Forms or WPF UI, you can set the `IWebScale.EventSynchronizationContext` property.

### Tare

The scale automatically resets itself to zero when it powers on. You can also manually tare it by calling `Tare()`.

```cs
await webScale.Tare();
```

After this task completes, the scale's weight will read zero.

### Dispose

When you are done with this instance, call [`Dispose()`](https://learn.microsoft.com/en-us/dotnet/api/system.idisposable.dispose) to disconnect from the device. You can also use a [`using` statement](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-statement) or [declaration](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-statement).

```cs
public void ExplicitlyDispose() {
    IWebScale webScale = new WebScale();
    // use webScale here
    webScale.Dispose();
}
```

```cs
public void ImplicitlyDisposeWithUsingDeclaration() {
    using IWebScale webScale = new WebScale();
    // use webScale here
    // when control exits the ImplicitlyDisposeWithUsingDeclaration method, webScale will be disposed
}
```

```cs
public void ImplicitlyDisposeWithUsingStatement() {
    using (IWebScale webScale = new WebScale()) {
        // use webScale here
        // when control exits the using block, webScale will be disposed
    }
}
```

## Sample

The [sample application](https://github.com/Aldaviva/WebScale.net/blob/master/Sample/Program.cs) demonstrates usage of this library.

You may clone this repository and run the sample with `dotnet run` in the `Sample` project directory.

You may also [download prebuilt executables](https://github.com/Aldaviva/WebScale.net/releases/latest/download/Samples.zip) of the sample app for Windows, Linux, and MacOS, each on x64 and ARM. Remember that the Linux app needs to be run with `sudo`, and the MacOS and Linux apps need the `chmod +x` executable bit.

## See also
- [**Aldaviva/webscale**](https://github.com/Aldaviva/webscale) - Node.js version of this library
