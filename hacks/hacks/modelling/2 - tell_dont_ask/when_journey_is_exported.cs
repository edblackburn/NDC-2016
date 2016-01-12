using System;
using NUnit.Framework;

namespace hacks.modelling.tell_dont_ask
{
    [TestFixture]
    public class when_journey_is_exported
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

            dynamic export = jny.Export();

            Assert.That(export.AccountId,Is.EqualTo(accountId));
            Assert.That(export.JourneyId, Is.EqualTo(id));
            Assert.That(export.Origin, Is.EqualTo(bank));
            Assert.That(export.Destination, Is.EqualTo(princeRegent));
            Assert.That(export.Fare, Is.EqualTo(10));
        }
    }
}