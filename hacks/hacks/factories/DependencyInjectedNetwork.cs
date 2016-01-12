using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using hacks.entities.value_objects;

namespace hacks.factories
{
    public class DependencyInjectedNetwork : INetwork
    {
        private readonly INetwork[] _networks;
        private readonly TextWriter _logger;

        public DependencyInjectedNetwork(INetwork[] networks, TextWriter logger)
        {
            _networks = networks;
            _logger = logger;
        }

        public short GetFare(OriginDestination originDestination, string mode)
        {
            var network = _networks
                .OfType<ISatisfyMode>()
                .SingleOrDefault(m => m.Matches(mode)) as INetwork ?? new InvalidMode(_networks, _logger);

            return network.GetFare(originDestination, mode);
        }
    }

    public class Bus : INetwork, ISatisfyMode
    {
        public Bus(/* adaptor to bus fare lookup system */)
        {
            
        }

        public short GetFare(OriginDestination originDestination, string mode)
        {
            return 10;
        }

        public bool Matches(string mode)
        {
            return string.Equals("bus", mode, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public class Rail : INetwork, ISatisfyMode
    {
        public Rail(/* adaptor to rail fare lookup system */)
        {
            
        }
        public short GetFare(OriginDestination originDestination, string mode)
        {
            return 5;
        }

        public bool Matches(string mode)
        {
            return string.Equals("rail", mode, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    public interface ISatisfyMode
    {
        bool Matches(string mode);
    }

    public interface INetwork
    {
        short GetFare(OriginDestination originDestination, string mode);
    }

    internal class InvalidMode : INetwork
    {
        private readonly IEnumerable<INetwork> _networks;
        private readonly TextWriter _logger;

        internal InvalidMode(IEnumerable<INetwork> networks, TextWriter logger)
        {
            _networks = networks;
            _logger = logger;
        }

        public short GetFare(OriginDestination originDestination, string mode)
        {
            const string format = "Cannot match {0} against {1} for origin destination {2}";

            var networks = string.Join(Environment.NewLine, _networks.Select(n => n.GetType().FullName));
            var message = string.Format(format, mode, networks, originDestination);

            _logger.WriteLine(message);

            return 0;
        }
    }

}