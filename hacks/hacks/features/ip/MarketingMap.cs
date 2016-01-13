using System;
using blazey.features;

namespace hacks.features.ip
{
    public class MarketingMap : IFeatureMap
    {
        private readonly Coin _coin;

        public MarketingMap()
        {
            _coin = new Coin();
        }

        public Type ImplementationType()
        {
            return _coin.Flip() == 0
                ? typeof (MarketingStrategyB)
                : typeof (MarketingStrategyB);
        }

        public Type FeatureType => typeof (IMarketingStrategy);
    }
}