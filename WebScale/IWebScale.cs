using HidClient;
using UnitsNet;

namespace Aldaviva.WebScale;

public interface IWebScale: IHidClient {

    event EventHandler<Force> WeightChanged;

    Force Weight { get; }

    Task Tare();

}