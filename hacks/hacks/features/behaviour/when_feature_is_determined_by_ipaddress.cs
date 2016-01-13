using blazey.features;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace hacks.features.behaviour
{
    [TestFixture]
    public class when_feature_is_determined_by_ipaddress
    {
        [Test]
        public void should_resolve_to_experimental()
        {
            var container = new WindsorContainer()
                .Register(
                    Component.For<IFeature>().ImplementedBy<Experimental>().LifestyleTransient(),
                    Component.For<IFeature>().ImplementedBy<DefaultBehaviour>().LifestyleTransient(),
                    Component.For<IHttpContextAccessor>().ImplementedBy<HttpContextAccessor>().LifestyleTransient());

            Features.Configure(container, c =>
                c.UseFeatureMap<IFeature, EarlyAccessMap>());

            var market = container.Resolve<IFeature>();

            Assert.That(market, Is.InstanceOf<Experimental>());
        }
    }
}