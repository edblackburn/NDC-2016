using System;
using blazey.substituter;
using Castle.Windsor;
using hacks.modelling.messaging;
using hacks.modelling.value_objects;
using NUnit.Framework;

namespace hacks.factories.specs
{
    [TestFixture]
    public class when_journey_recieves_origin_to_destination_tap_message
    {
        [Test]
        public void should_be_bank_to_prince_regent()
        {
            const string bank = "Bank";
            const string princeRegent = "Prince Regent";

            var accountId = Guid.NewGuid();
            var tappedAtBankCommand = new DeviceTappedCommand(accountId, bank, "rail");
            var tappedAtprinceRegentCommand = new DeviceTappedCommand(accountId, princeRegent, "rail");
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
            handleDevice.Handle(tappedAtprinceRegentCommand);
            
            Assert.That(accountSpy[accountId].OriginDestination,
                Is.EqualTo(OriginDestination.OriginToDestination(bank, princeRegent)));
            Assert.That(accountSpy[accountId].Fare, Is.EqualTo(5));

            container.Release(handleDevice);
        }

        [Test]
        public void should_not_rate_for_invalid_journey()
        {
            const string bank = "Bank";
            var accountId = Guid.NewGuid();
            var firstBankTapCommand = new DeviceTappedCommand(accountId, bank, "rail");
            var secondBankTapCommand = new DeviceTappedCommand(accountId, bank, "hyperloop");
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
           
            Assert.That(accountSpy[accountId].OriginDestination, 
                Is.EqualTo(OriginDestination.HereToHere(bank)));

            Assert.That(accountSpy[accountId].Fare, Is.EqualTo(0));
            
            container.Release(handleDevice);
        }

        [Test]
        public void should_be_bank_to_bank()
        {
            const string bank = "Bank";
            var accountId = Guid.NewGuid();
            var firstBankTapCommand = new DeviceTappedCommand(accountId, bank, "rail");
            var secondBankTapCommand = new DeviceTappedCommand(accountId, bank, "rail");
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

            var projection = accountSpy.Get(accountId).Project();

            Assert.That(projection.OriginDestination, Is.EqualTo(OriginDestination.HereToHere(bank)));
            Assert.That(projection.Fare, Is.EqualTo(5));


            container.Release(handleDevice);
        }
    }
}