using Aldaviva.WebScale;

Console.WriteLine("Connecting to web scale...");
using IWebScale webScale = new WebScale();
webScale.EventSynchronizationContext.Post(_ => { }, null);
webScale.WeightChanged += (_, weight) => Console.WriteLine($"{weight.OunceForce,4:F1} oz.");

Console.WriteLine("Taring web scale...");
await webScale.Tare();
Console.WriteLine("Tared web scale.");

Console.WriteLine($"Initial weight: {webScale.Weight.OunceForce,4:F1} oz.");

Console.WriteLine("Waiting for weight to change. Press Ctrl+C to exit.");
CancellationTokenSource cts = new();
Console.CancelKeyPress += (_, eventArgs) => {
    eventArgs.Cancel = true;
    cts.Cancel();
};
cts.Token.WaitHandle.WaitOne();