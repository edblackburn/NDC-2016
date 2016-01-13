using blazey.features;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace hacks.features.ip
{
    [TestFixture]
    public class when_feature_is_split
    {
        [Test]
        public void should_split_strategies()
        {
            var container = new WindsorContainer()
                .Register(
                    Component.For<IMarketingStrategy>().ImplementedBy<MarketingStrategyA>(),
                    Component.For<IMarketingStrategy>().ImplementedBy<MarketingStrategyB>());

            Features.Configure(container, c =>
                c.UseFeatureMap<IMarketingStrategy, MarketingMap>());

            var market = container.Resolve<IMarketingStrategy>();

            Assert.That(market, Is.InstanceOf<MarketingStrategyB>());
        }
    }
}