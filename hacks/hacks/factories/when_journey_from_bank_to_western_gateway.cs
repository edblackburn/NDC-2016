using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using blazey.substituter;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using hacks.entities.value_objects;
using hacks.messaging;
using NUnit.Framework;
using Journey = hacks.messaging.Journey;

namespace hacks.factories
{
    [TestFixture]
    public class when_journey_recieves_origin_to_destination_tap_message
    {
        [Test]
        public void should_be_bank_to_western_gate()
        {
            const string bank = "Bank";
            const string westernGateway = "Western Gateway";

            var accountId = Guid.NewGuid();
            var tappedAtBankCommand = new DeviceTapped(accountId, bank, "rail");
            var tappedAtWesternGatewayCommand = new DeviceTapped(accountId, westernGateway, "rail");
            var accountSpy = new AccountSpy();

            var container = new WindsorContainer();
            container.AddFacility<SubstituterFacility>(config =>
            {
                config.WithContainer(container)
                    .Substitute<IAccount>(sub => sub.Instance(accountSpy));
            });

            container.Install(new InstallDevices());

            var handleDevice = container.Resolve<HandleDevice>();

            handleDevice.Handle(tappedAtBankCommand);
            handleDevice.Handle(tappedAtWesternGatewayCommand);

            var jny = accountSpy.Get(accountId).Export();

            Assert.That(jny.OriginDestination,
                Is.EqualTo(OriginDestination.OriginToDestination(bank, westernGateway)));
            Assert.That(jny.Fare, Is.EqualTo(5));

            container.Release(handleDevice);
        }

        [Test]
        public void should_not_rate_for_invalid_journey()
        {
            const string bank = "Bank";
            var accountId = Guid.NewGuid();
            var firstBankTapCommand = new DeviceTapped(accountId, bank, "rail");
            var secondBankTapCommand = new DeviceTapped(accountId, bank, "hyperloop");
            var accountSpy = new AccountSpy();

            var container = new WindsorContainer();
            container.AddFacility<SubstituterFacility>(config =>
            {
                config.WithContainer(container)
                    .Substitute<IAccount>(sub => sub.Instance(accountSpy));
            });

            container.Install(new InstallDevices());

            var handleDevice = container.Resolve<HandleDevice>();

            handleDevice.Handle(firstBankTapCommand);
            handleDevice.Handle(secondBankTapCommand);

            var jny = accountSpy.Get(accountId).Export();

            Assert.That(jny.OriginDestination, Is.EqualTo(OriginDestination.HereToHere(bank)));
            Assert.That(jny.Fare, Is.EqualTo(0));


            container.Release(handleDevice);
        }

        [Test]
        public void should_be_bank_to_bank()
        {
            const string bank = "Bank";
            var accountId = Guid.NewGuid();
            var firstBankTapCommand = new DeviceTapped(accountId, bank, "rail");
            var secondBankTapCommand = new DeviceTapped(accountId, bank, "rail");
            var accountSpy = new AccountSpy();

            var container = new WindsorContainer();
            container.AddFacility<SubstituterFacility>(config =>
            {
                config.WithContainer(container)
                    .Substitute<IAccount>(sub => sub.Instance(accountSpy));
            });

            container.Install(new InstallDevices());

            var handleDevice = container.Resolve<HandleDevice>();

            handleDevice.Handle(firstBankTapCommand);
            handleDevice.Handle(secondBankTapCommand);

            var jny = accountSpy.Get(accountId).Export();

            Assert.That(jny.OriginDestination, Is.EqualTo(OriginDestination.HereToHere(bank)));
            Assert.That(jny.Fare, Is.EqualTo(5));


            container.Release(handleDevice);
        }

    }

    internal class AccountSpy : IAccount
    {
        private readonly IDictionary<Guid, Journey> _journeys = new Dictionary<Guid, Journey>();

        public Journey Get(Guid id)
        {
            return _journeys.ContainsKey(id) ? _journeys[id] : new Journey(id);
        }

        public void Store(Journey journey)
        {
            _journeys[journey.AccountId] = journey;
        }
    }

    public class HandleDevice
    {
        private readonly INetwork _network;
        private readonly IAccount _account;

        public HandleDevice(DependencyInjectedNetwork network, IAccount account)
        {
            _network = network;
            _account = account;
        }

        public void Handle(DeviceTapped deviceTapped)
        {
            var jny = _account.Get(deviceTapped.AccountId);
            jny.RecieveTap(deviceTapped);
            jny.AssignFare(od => _network.GetFare(od, deviceTapped.Mode));
            _account.Store(jny);
        }
    }

    public interface IAccount
    {
        Journey Get(Guid id);
        void Store(Journey journey);
    }

    public class InstallDevices : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver.AddSubResolver(new ArrayResolver(container.Kernel));
            container.Register(Component.For<TextWriter>().Instance(Console.Out));
            container.Register(Classes.FromThisAssembly()
                .InSameNamespaceAs<HandleDevice>()
                .WithService.AllInterfaces()
                .WithService.Self()
                .LifestyleTransient());
        }
    }
}