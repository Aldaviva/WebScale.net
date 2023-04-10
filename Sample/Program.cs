using Aldaviva.WebScale;

Console.WriteLine("Connecting to scale...");
using IWebScale webScale = new WebScale();

webScale.WeightChanged += (_, weight) => Console.WriteLine($"{weight.OunceForce,4:N1} oz.");

webScale.IsConnectedChanged += (_, isConnected) => Console.WriteLine($"Scale {(isConnected ? "connected" : "disconnected")}");

if (webScale.IsConnected) {
    Console.WriteLine("Connected. Taring scale...");
    await webScale.Tare();
    Console.WriteLine($"Initial weight: {webScale.Weight.OunceForce,4:N1} oz.");
    Console.WriteLine("Waiting for weight to change. Press Ctrl+C to exit.");
} else {
    Console.WriteLine("No scale detected. Waiting for scale to connect. Press Ctrl+C to exit.");
}

CancellationTokenSource cancellationTokenSource = new();
Console.CancelKeyPress += (_, eventArgs) => {
    eventArgs.Cancel = true;
    cancellationTokenSource.Cancel();
};
cancellationTokenSource.Token.WaitHandle.WaitOne();