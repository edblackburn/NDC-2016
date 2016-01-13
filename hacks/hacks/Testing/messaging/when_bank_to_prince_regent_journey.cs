using System;
using blazey.substituter;
using Castle.Windsor;
using hacks.factories;
using hacks.factories.specs;
using hacks.modelling.messaging;
using hacks.modelling.value_objects;
using NUnit.Framework;
using NUnit.Specifications;

namespace hacks.testing.messaging
{
    public class when_journey_recieves_origin_to_destination_tap_message : ContextSpecification
    {
        private Establish that = () =>
        {
            _bank = "Bank";
            _princeRegent = "Prince Regent";

            _accountId = Guid.NewGuid();
            _tappedAtBankCommand = new DeviceTappedCommand(_accountId, _bank, "rail");
            _tappedAtprinceRegentCommand = new DeviceTappedCommand(_accountId, _princeRegent, "rail");
            _accountSpy = new AccountSpy();

            _container = new WindsorContainer();
            _container.AddFacility<SubstituterFacility>(config =>
            {
                config.WithContainer(_container)
                    .Substitute<IAccount>(sub => sub.Instance(_accountSpy));
            });

            _container.Install(new InstallDevices());

            _handleDevice = _container.Resolve<HandleDevice>();
        };

        private Because handle_taps = () =>
        {
            _handleDevice.Handle(_tappedAtBankCommand);
            _handleDevice.Handle(_tappedAtprinceRegentCommand);
        };

        private It should_be_have_fare = () =>
            Assert.That(_accountSpy[_accountId].Fare, Is.EqualTo(5));

        private It should_be_bank_to_prince_regent = () =>
            Assert.That(_accountSpy[_accountId].OriginDestination,
                Is.EqualTo(OriginDestination.OriginToDestination(_bank, _princeRegent)));

        private Cleanup release = () => _container.Release(_handleDevice);
        
        private static string _bank;
        private static string _princeRegent;
        private static IWindsorContainer _container;
        private static HandleDevice _handleDevice;
        private static DeviceTappedCommand _tappedAtBankCommand;
        private static DeviceTappedCommand _tappedAtprinceRegentCommand;
        private static AccountSpy _accountSpy;
        private static Guid _accountId;
    }
}