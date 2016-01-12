using System;
using hacks.modelling.value_objects;

namespace hacks.factories.modes
{
    public class Rail : INetwork, ISatisfyMode
    {
        public short GetFare(OriginDestination originDestination, string mode)
        {
            return 5;
        }

        public bool Matches(string mode)
        {
            return string.Equals("rail", mode, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}