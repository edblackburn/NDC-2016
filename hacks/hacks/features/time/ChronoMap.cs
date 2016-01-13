using System;
using blazey.features;

namespace hacks.features.time
{
    public class ChronoMap : IFeatureMap
    {
        private readonly Now _now;

        public ChronoMap(Now now)
        {
            _now = now;
        }

        public Type ImplementationType()
        {
            var releaseDate = new DateTimeOffset(2016, 2, 2, 13, 0, 0, TimeSpan.Zero);
            return releaseDate >= _now() ? typeof (NewFeature) : typeof (OldFeature);
        }

        public Type FeatureType => typeof (IFeature);
    }
}