using System;
using hacks.entities.value_objects;

namespace hacks.factories
{
    internal class SwitchNetwork : INetwork
    {
        public short GetFare(OriginDestination originDestination, string mode)
        {
            switch (mode)
            {
                case "rail":
                    return 10;
                case "bus":
                    return 2;
                default:
                    throw new InvalidOperationException($"Cannot get fare for mode {mode}");
            }
        }
    }
}