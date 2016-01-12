using System.Linq;
using hacks.factories.modes;
using hacks.modelling.value_objects;

namespace hacks.factories.networks
{
    public class DependencyInjectedNetwork : INetwork
    {
        private readonly INetwork[] _networks;

        public DependencyInjectedNetwork(INetwork[] networks)
        {
            _networks = networks;
        }

        public short GetFare(OriginDestination originDestination, string mode)
        {
            var network = _networks
                .OfType<ISatisfyMode>()
                .SingleOrDefault(m => m.Matches(mode)) as INetwork
                          ?? new InvalidMode(_networks);

            return network.GetFare(originDestination, mode);
        }
    }
}