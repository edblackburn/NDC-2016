using hacks.factories.networks;
using hacks.modelling.messaging;

namespace hacks.factories
{
    public class HandleDevice
    {
        private readonly INetwork _network;
        private readonly IAccount _account;

        public HandleDevice(DependencyInjectedNetwork network, IAccount account)
        {
            _network = network;
            _account = account;
        }

        public void Handle(DeviceTappedCommand deviceTappedCommand)
        {
            var jny = _account.Get(deviceTappedCommand.AccountId);
            jny.RecieveTap(deviceTappedCommand);
            jny.AssignFare(od => _network.GetFare(od, deviceTappedCommand.Mode));
            _account.Store(jny);
        }
    }
}