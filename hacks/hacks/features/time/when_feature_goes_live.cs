using System;
using blazey.features;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace hacks.features.time
{
    [TestFixture]
    public class when_feature_goes_live
    {
        [Test]
        public void should_resolve_to_experimental()
        {
            var container = new WindsorContainer()
                .Register(
                    Component.For<IFeature>().ImplementedBy<OldFeature>().LifestyleTransient(),
                    Component.For<IFeature>().ImplementedBy<NewFeature>().LifestyleTransient(),
                    Component.For<Now>()
                        .Instance(() => new DateTimeOffset(2016, 1, 1, 0, 0, 0, TimeSpan.Zero))
                        .LifestyleTransient());

            Features.Configure(container, c =>
                c.UseFeatureMap<IFeature, ChronoMap>());

            var market = container.Resolve<IFeature>();

            Assert.That(market, Is.InstanceOf<OldFeature>());
        }
    }
}