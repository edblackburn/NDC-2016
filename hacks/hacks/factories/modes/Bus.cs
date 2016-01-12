using System;
using hacks.modelling.value_objects;

namespace hacks.factories.modes
{
    public class Bus : INetwork, ISatisfyMode
    {
        public short GetFare(OriginDestination originDestination, string mode)
        {
            return 10;
        }

        public bool Matches(string mode)
        {
            return string.Equals("bus", mode, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}