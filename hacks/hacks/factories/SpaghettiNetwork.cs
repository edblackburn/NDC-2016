using System;
using hacks.entities.value_objects;

namespace hacks.factories
{
    internal class SpaghettiNetwork : INetwork
    {
        public short GetFare(OriginDestination originDestination, string mode)
        {
            if (mode == "rail")
            {
                return 10;
            }

            if (mode == "bus")
            {
                return 2;
            }

            throw new InvalidOperationException($"Cannot get fare for mode {mode}");
        }
    }
}