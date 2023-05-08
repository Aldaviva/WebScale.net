using HidClient;
using HidSharp;
using UnitsNet;

namespace Aldaviva.WebScale;

/// <summary>
/// <para>Primary class of the <c>WebScale</c> package.</para>
/// <para>To get started, construct a new instance of <see cref="WebScale"/>, then listen for the <see cref="WeightChanged"/> event.</para>
/// <para>Example:</para>
/// <para><c>using Aldaviva.WebScale;
///
/// using IWebScale webScale = new WebScale();
/// webScale.WeightChanged += (sender, weight) => Console.WriteLine($"{weight.OunceForce,4:F1} oz.");</c></para>
/// </summary>
public class WebScale: AbstractHidClient, IWebScale {

    private static readonly byte[] TareCommand = { 0x04, 0x01 };

    /// <inheritdoc />
    protected override int VendorId { get; } = 0x2474;

    /// <inheritdoc />
    protected override int ProductId { get; } = 0x0550;

    /// <inheritdoc />
    public event EventHandler<Force>? WeightChanged;

    private Force                    _weight;
    private CancellationTokenSource? _disposalTokenSource = new();

    private readonly SemaphoreSlim _taring = new(1);

    /// <summary>
    /// <para>Create a new instance by finding a device attached to the local computer.</para>
    /// <para>After constructing it, you can listen for <see cref="WeightChanged"/> events.</para>
    /// <para>Remember to <see cref="Dispose"/> of this instance to disconnect from the device.</para>
    /// </summary>
    public WebScale() { }

    /// <summary>
    /// <para>For normal library usage, you should call <see cref="WebScale()"/> instead.</para>
    /// <para>Create a new instance using a custom list of devices. Useful for unit test mocking.</para>
    /// <para>After constructing it, you can listen for <see cref="WeightChanged"/> events.</para>
    /// <para>Remember to <see cref="Dispose"/> of this instance to disconnect from the device.</para>
    /// </summary>
    public WebScale(DeviceList deviceList): base(deviceList) { }

    /// <inheritdoc />
    public Force Weight {
        get => _weight;
        internal set {
            bool isTaring = _taring.CurrentCount == 0;
            bool isZeroed = value.Equals(Force.Zero, 0.05, ComparisonType.Absolute);
            bool isChange = !_weight.Equals(value, 0.05, ComparisonType.Absolute);

            if (isTaring && isZeroed) {
                try {
                    // allow the waiting call to Tare() to proceed
                    _taring.Release();
                } catch (SemaphoreFullException) { }
            }

            if (isChange && (!isTaring || isZeroed)) {
                _weight = value;
                EventSynchronizationContext.Post(OnWeightChanged, _weight);
            }
        }
    }

    private void OnWeightChanged(object weight) {
        WeightChanged?.Invoke(this, (Force) weight);
        OnPropertyChanged();
    }

    /// <inheritdoc />
    protected override void OnHidRead(byte[] readBuffer) {
        // Console.WriteLine($"Read HID bytes {string.Join("", readBuffer.Select(b => $"{b:x2}"))}");
        Weight = Force.FromOunceForce(BitConverter.ToInt16(readBuffer, 4) / 10.0);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing) {
        if (disposing) {
            try {
                _disposalTokenSource?.Cancel(false);
            } catch (AggregateException) { }

            _disposalTokenSource?.Dispose();
            _disposalTokenSource = null;
            _taring.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public async Task Tare() {
        if (DeviceStream is not null && _disposalTokenSource?.Token is { } disposalToken) {
            try {
                // if another call to Tare() has not yet finished, wait for it here
                await _taring.WaitAsync(disposalToken).ConfigureAwait(false);

                await DeviceStream.WriteAsync(TareCommand, 0, TareCommand.Length, disposalToken).ConfigureAwait(false);

                // wait for OnHidRead() to set Weight to 0, which means we're done taring
                await _taring.WaitAsync(disposalToken).ConfigureAwait(false);
                try {
                    // allow other queued calls to Tare() to proceed
                    _taring.Release();
                } catch (SemaphoreFullException) { }
            } catch (OperationCanceledException) { }
        }
    }

}