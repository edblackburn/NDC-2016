using hacks.modelling.value_objects;
using NUnit.Framework;

namespace hacks.modelling.messaging.specs
{
    [TestFixture]
    public class when_journey_recieves_origin_to_destination_tap_message
    {
        [Test]
        public void should_be_bank_to_prince_regent()
        {
            const string bank = "Bank";
            const string princeRegent = "Prince Regent";
            const short fare = 10;

            var journey = new Journey();

            var msg1 = new DeviceTappedCommand(journey.AccountId, bank, "rail");
            var msg2 = new DeviceTappedCommand(journey.AccountId, princeRegent, "rail");

            journey.RecieveTap(msg2);
            journey.RecieveTap(msg1);

            journey.AssignFare(od => fare);

            Assert.That(journey.Project().OriginDestination,
                Is.EqualTo(OriginDestination.OriginToDestination(bank, princeRegent)));
            Assert.That(journey.Project().Fare, Is.EqualTo(10));
        }

        [Test]
        public void should_be_bank_to_bank()
        {
            const string bank = "Bank";
            const short fare = 10;

            var journey = new Journey();

            var msg1 = new DeviceTappedCommand(journey.AccountId, bank, "rail");
            var msg2 = new DeviceTappedCommand(journey.AccountId, bank, "rail");

            journey.RecieveTap(msg2);
            journey.RecieveTap(msg1);

            journey.AssignFare(od => fare);

            Assert.That(journey.Project().OriginDestination, Is.EqualTo(OriginDestination.HereToHere(bank)));
            Assert.That(journey.Project().Fare, Is.EqualTo(10));
        }
    }
}