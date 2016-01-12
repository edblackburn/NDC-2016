using System;
using System.IO;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace hacks.factories
{
    public class InstallDevices : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));

            container.Register(Classes.FromThisAssembly()
                .InSameNamespaceAs<HandleDevice>(true)
                .WithService.AllInterfaces()
                .WithService.Self()
                .LifestyleTransient());
        }
    }
}