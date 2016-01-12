using System;
using hacks.modelling.value_objects;
using NUnit.Framework;

namespace hacks.modelling.messaging.specs
{
    [TestFixture]
    public class when_journey_recieves_here_to_here_tap_message
    {
        [Test]
        public void should_be_bank_to_bank()
        {
            const string bank = "Bank";
            const short fare = 10;

            var journey = new Journey();

            var msg1 = new DeviceTappedCommand(journey.AccountId, bank, "rail");

            journey.RecieveTap(msg1);

            journey.AssignFare(od => fare);

            dynamic projection = journey.Project();

            Assert.That(projection.OriginDestination, Is.EqualTo(OriginDestination.HereToHere(bank)));
            Assert.That(projection.Fare, Is.EqualTo(10));
        }
    }
}