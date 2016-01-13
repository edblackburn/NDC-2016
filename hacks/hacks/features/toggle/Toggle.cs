using System;
using blazey.features;

namespace hacks.features.toggle
{
    public class Toggle : IFeatureMap
    {
        public Type ImplementationType()
        {
            return typeof (On);
        }

        public Type FeatureType => typeof (IFeature);
    }
}