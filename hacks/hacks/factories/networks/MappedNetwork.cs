using System;
using System.Collections.Generic;
using hacks.modelling.value_objects;

namespace hacks.factories.networks
{
    internal class MappedNetwork : INetwork
    {
        public short GetFare(OriginDestination originDestination, string mode)
        {
            var map = new Dictionary<string, Func<OriginDestination, short>>
            {
                {"rail", od => 10},
                {"bus", od => 10}
            };

            if (map.ContainsKey(mode))
            {
                return map[mode](originDestination);
            }

            throw new InvalidOperationException($"Cannot get fare for mode {mode}");
        }
    }
}