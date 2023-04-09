using HidClient;
using HidSharp;
using UnitsNet;

namespace Aldaviva.WebScale;

public class WebScale: AbstractHidClient, IWebScale {

    private static readonly byte[] TareCommand = { 0x04, 0x01 };

    protected override int VendorId { get; } = 0x2474;
    protected override int ProductId { get; } = 0x0550;

    /// <inheritdoc />
    public event EventHandler<Force>? WeightChanged;

    private Force                    _weight;
    private CancellationTokenSource? _cancellationTokenSource = new();

    private readonly SemaphoreSlim _taring = new(1);

    public WebScale() { }
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

    protected override void Dispose(bool disposing) {
        if (disposing) {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _taring.Dispose();
        }

        base.Dispose(disposing);
    }

    public async Task Tare() {
        if (DeviceStream is not null && _cancellationTokenSource?.Token is { } cancellationToken) {
            await _taring.WaitAsync(cancellationToken).ConfigureAwait(false);

            await DeviceStream.WriteAsync(TareCommand, 0, TareCommand.Length, cancellationToken).ConfigureAwait(false);

            await _taring.WaitAsync(cancellationToken).ConfigureAwait(false);
            try {
                _taring.Release();
            } catch (SemaphoreFullException) { }
        }
    }

}