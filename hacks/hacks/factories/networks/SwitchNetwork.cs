using System;
using hacks.modelling.value_objects;

namespace hacks.factories.networks
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