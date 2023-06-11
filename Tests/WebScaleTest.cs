using Aldaviva.WebScale;
using HidSharp;
using UnitsNet;

namespace Tests;

public class WebScaleTest {

    [Fact]
    public void Constructor() {
        new WebScale().Dispose();
    }

    [Theory]
    [InlineData("03040bff0000", 0.0)]
    [InlineData("03040bff0600", 0.6)]
    [InlineData("03040bff1900", 2.5)]
    [InlineData("03050bffedff", -1.9)]
    [InlineData("03050bff2201", 29.0)]
    public void Read(string hexHidData, double expectedOunces) {
        byte[] hidBytes = Convert.FromHexString(hexHidData);

        TestWebScale webScale = new(EmptyDeviceList.Instance);
        webScale.OnHidReadTest(hidBytes);

        webScale.Weight.OunceForce.Should().BeApproximately(expectedOunces, 0.01);
    }

    [Fact]
    public void NoChangeForSameWeight() {
        WebScale webScale = new(EmptyDeviceList.Instance);
        webScale.EventSynchronizationContext = new SynchronousSynchronizationContext();
        webScale.Weight                      = Force.FromOunceForce(1.0);

        int    changeEventCount      = 0;
        Force? mostRecentChangeEvent = null;

        webScale.WeightChanged += (_, newWeight) => {
            changeEventCount++;
            mostRecentChangeEvent = newWeight;
        };

        webScale.Weight = Force.FromOunceForce(2.0);
        webScale.Weight = Force.FromOunceForce(2.0);

        changeEventCount.Should().Be(1);
        mostRecentChangeEvent.Should().NotBeNull();
        mostRecentChangeEvent!.Value.Equals(Force.FromOunceForce(2.0), Force.FromOunceForce(0.05)).Should().BeTrue();
    }

    [Fact]
    public async Task TareCommand() {
        TestWebScale webScale = new(EmptyDeviceList.Instance);
        webScale.EventSynchronizationContext = new SynchronousSynchronizationContext();
        webScale.DeviceStreamTest            = A.Fake<HidStream>();

        int    changeEventCount      = 0;
        Force? mostRecentChangeEvent = null;

        webScale.WeightChanged += (_, newWeight) => {
            changeEventCount++;
            mostRecentChangeEvent = newWeight;
        };

        Task tare = webScale.Tare();

        webScale.Weight = Force.FromOunceForce(0.6);
        webScale.Weight = Force.FromOunceForce(0.6);
        webScale.Weight = Force.FromOunceForce(0.0);

        await tare;

        changeEventCount.Should().Be(0);
        mostRecentChangeEvent.Should().BeNull();
        webScale.Weight.Equals(Force.Zero, Force.FromOunceForce(0.05)).Should().BeTrue();

        A.CallTo(() => webScale.DeviceStreamTest.WriteAsync(A<byte[]>.That.IsSameSequenceAs(new byte[] { 0x04, 0x01 }), 0, 2, A<CancellationToken>._)).MustHaveHappened();
    }

    [Fact]
    public void TareIgnoredChangesUntilDone() { }

    private class TestWebScale: WebScale {

        public TestWebScale() { }
        public TestWebScale(DeviceList deviceList): base(deviceList) { }

        public HidStream? DeviceStreamTest {
            get => DeviceStream;
            set => DeviceStream = value;
        }

        internal void OnHidReadTest(byte[] readBuffer) {
            OnHidRead(readBuffer);
        }

    }

    private class SynchronousSynchronizationContext: SynchronizationContext {

        public override void Post(SendOrPostCallback d, object? state) {
            Send(d, state);
        }

    }

}