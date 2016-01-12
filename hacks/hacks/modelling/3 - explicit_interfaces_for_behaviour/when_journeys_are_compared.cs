using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace hacks.modelling.explicit_interfaces_for_behaviour
{
    [TestFixture]
    public class when_journeys_are_compared
    {
        [Test]
        public void should_be_sortable()
        {
            const string kingsCross = "Kings Cross";
            const string bank = "Bank";
            const string princeRegent = "Prince Regent";
            const short fare = 10;

            var accountId = Guid.NewGuid();

            var jny1 = new Journey(accountId, kingsCross, bank);
            var jny2 = new Journey(accountId, bank, princeRegent);
            var jny3 = new Journey(accountId, princeRegent, bank);

            jny1.AssignFare((o, d) => fare);
            jny2.AssignFare((o, d) => fare);
            jny3.AssignFare((o, d) => fare);

            var jourenys = new List<Journey>(new[] {jny2, jny3, jny1});
            jourenys.Sort();

            Assert.That(jourenys.First().Export().Origin, Is.EqualTo(kingsCross));
            Assert.That(jourenys.First().Export().Destination, Is.EqualTo(bank));
            Assert.That(jourenys.First().Export().Fare, Is.EqualTo(10));
            Assert.That(jourenys.First().Export().AccountId, Is.EqualTo(accountId));
            Assert.That(jourenys.First().Export().JourneyId, Is.Not.EqualTo(Guid.Empty));

            Assert.That(jourenys.ElementAt(1).Export().Origin, Is.EqualTo(bank));
            Assert.That(jourenys.ElementAt(1).Export().Destination, Is.EqualTo(princeRegent));
            Assert.That(jourenys.ElementAt(1).Export().Fare, Is.EqualTo(10));
            Assert.That(jourenys.ElementAt(1).Export().AccountId, Is.EqualTo(accountId));
            Assert.That(jourenys.ElementAt(1).Export().JourneyId, Is.Not.EqualTo(Guid.Empty));

            Assert.That(jourenys.Last().Export().Origin, Is.EqualTo(princeRegent));
            Assert.That(jourenys.Last().Export().Destination, Is.EqualTo(bank));
            Assert.That(jourenys.Last().Export().Fare, Is.EqualTo(10));
            Assert.That(jourenys.Last().Export().AccountId, Is.EqualTo(accountId));
            Assert.That(jourenys.Last().Export().JourneyId, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void should_equate()
        {
            const string bank = "Bank";
            const string princeRegent = "Prince Regent";

            var accountId = Guid.NewGuid();

            var jny1 = new Journey(accountId, bank, princeRegent);
            var jny2 = new Journey(accountId, princeRegent, bank);
            var jny3 = jny1;

            Assert.That(jny1, Is.EqualTo(jny1));
            Assert.That(jny1, Is.Not.EqualTo(jny2));
            Assert.That(jny3, Is.Not.EqualTo(jny2));
            Assert.That(jny3, Is.EqualTo(jny1));
        }
    }
}