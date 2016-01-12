using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using hacks.modelling.value_objects;

namespace hacks.factories.modes
{
    internal class InvalidMode : INetwork, ISatisfyMode
    {
        private readonly IEnumerable<INetwork> _networks;

        internal InvalidMode(IEnumerable<INetwork> networks)
        {
            _networks = networks;
        }

        public short GetFare(OriginDestination originDestination, string mode)
        {
            const string format = "Cannot match {0} against {1} for origin destination {2}";

            var networks = string.Join(Environment.NewLine, _networks.Select(n => n.GetType().FullName));
            var message = string.Format(format, mode, networks, originDestination);

            Console.WriteLine(message);

            return 0;
        }

        public bool Matches(string mode)
        {
            return false;
        }
    }
}