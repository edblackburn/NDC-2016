using blazey.features;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace hacks.features.toggle
{
    [TestFixture]
    public class when_feature_is_toggled_off
    {
        [Test]
        public void should_resolve_to_experimental()
        {
            var container = new WindsorContainer()
                .Register(
                    Component.For<IFeature>().ImplementedBy<Off>().LifestyleTransient(),
                    Component.For<IFeature>().ImplementedBy<On>().LifestyleTransient());

            Features.Configure(container, c =>
                c.UseFeatureMap<IFeature, Toggle>());

            var market = container.Resolve<IFeature>();

            Assert.That(market, Is.InstanceOf<On>());
        }
    }
}