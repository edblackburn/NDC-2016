using hacks.modelling.value_objects;
using NUnit.Framework;

namespace hacks.modelling.messaging.specs
{
    [TestFixture]
    public class when_journey_recieves_no_tap
    {
        [Test]
        public void should_have_no_fare()
        {
            const short fare = 10;

            var journey = new Journey();

            journey.AssignFare(od => fare);

            Assert.That(OriginDestination.HasNoJourney(journey.Project().OriginDestination), Is.True);
            Assert.That(journey.Project().Fare, Is.EqualTo(0));
        }
    }
}