using System;
using NUnit.Framework;

namespace hacks.modelling.tell_dont_ask
{
    [TestFixture]
    public class when_journey_is_projected
    {
        [Test]
        public void should_incude_fields()
        {
            const string bank = "Bank";
            const string princeRegent = "Prince Regent";
            const short fare = 10;

            var id = Guid.NewGuid();
            var accountId = Guid.NewGuid();
            var jny = new Journey(id, accountId, bank, princeRegent);

            jny.AssignFare((origin, destination) => fare);

            dynamic projection = jny.Project();

            Assert.That(projection.AccountId,Is.EqualTo(accountId));
            Assert.That(projection.JourneyId, Is.EqualTo(id));
            Assert.That(projection.Origin, Is.EqualTo(bank));
            Assert.That(projection.Destination, Is.EqualTo(princeRegent));
            Assert.That(projection.Fare, Is.EqualTo(10));
        }
    }
}