using HidClient;
using UnitsNet;

namespace Aldaviva.WebScale;

/// <summary>
/// <para>Interface for the primary class of the <c>WebScale</c> package. Useful for decoupling implementations and test mocking.</para>
/// <para>To get started, construct a new instance of <see cref="WebScale"/>, then listen for the <see cref="WeightChanged"/> event.</para>
/// <para>Example:</para>
/// <para><c>using Aldaviva.WebScale;
///
/// using IWebScale webScale = new WebScale();
/// webScale.WeightChanged += (sender, weight) => Console.WriteLine($"{weight.OunceForce,4:F1} oz.");</c></para>
/// </summary>
public interface IWebScale: IHidClient {

    /// <summary>
    /// <para>The current weight reading of the scale, matching the value shown on the device's LCD. Precision is tenths of an ounce.</para>
    /// <para>When connected to a scale, this property will automatically update every 400 ± 15 milliseconds, although it can be slower when the weight is changing and the scale is waiting for its reading to converge before reporting it.</para>
    /// <para>To receive notifications when the weight changes, subscribe to either the <see cref="WeightChanged"/> or <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/> events.</para>
    /// </summary>
    Force Weight { get; }

    /// <summary>
    /// <para>Fired when the weight on the scale changes by at least 0.1 ounces. Not fired while taring.</para>
    /// <para>Event contains the new value of <see cref="Weight"/>.</para>
    /// </summary>
    event EventHandler<Force> WeightChanged;

    /// <summary>
    /// <para>Reset the amount of weight currently read by the scale to zero.</para>
    /// <para>The scale will also automatically tare itself whenever it is powered on without you calling this method.</para>
    /// </summary>
    Task Tare();

}