using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace hacks.modelling.memento
{
    [TestFixture]
    public class when_journeys_are_compared
    {
        [Test]
        public void should_be_sortable()
        {
            const string bank = "Bank";
            const string princeRegent = "Prince Regent";
            const short fare = 10;

            var jny1Memento = new JourneyMemento
            {
                Origin = bank,
                Destination = princeRegent,
                Fare = 0
            };

            var jny2Memento = new JourneyMemento
            {
                Origin = princeRegent,
                Destination = bank,
                Fare = 0
            };

            var jny1 = new Journey(jny1Memento);
            var jny2 = new Journey(jny2Memento);

            jny1.AssignFare((o, d) => fare);
            jny2.AssignFare((o, d) => fare);

            var journeys = new List<Journey>(new[] {jny2, jny1});
            journeys.Sort();

            Assert.That(journeys.First().Project().Origin, Is.EqualTo(bank));
            Assert.That(journeys.First().Project().Destination, Is.EqualTo(princeRegent));
            Assert.That(journeys.First().Project().Fare, Is.EqualTo(10));

            Assert.That(journeys.Last().Project().Origin, Is.EqualTo(princeRegent));
            Assert.That(journeys.Last().Project().Destination, Is.EqualTo(bank));
            Assert.That(journeys.Last().Project().Fare, Is.EqualTo(10));
        }

        [Test]
        public void should_equate()
        {
            const string bank = "Bank";
            const string princeRegent = "Prince Regent";

            var jny1Memento = new JourneyMemento
            {
                Origin = bank,
                Destination = princeRegent,
                Fare = short.MinValue
            };

            var jny2Memento = new JourneyMemento
            {
                Origin = princeRegent,
                Destination = bank,
                Fare = short.MaxValue
            };
            var jny1 = new Journey(jny1Memento);
            var jny2 = new Journey(jny2Memento);
            var jny3 = jny1;

            Assert.That(jny1, Is.EqualTo(jny1));
            Assert.That(jny1, Is.Not.EqualTo(jny2));
            Assert.That(jny3, Is.Not.EqualTo(jny2));
            Assert.That(jny3, Is.EqualTo(jny1));
        }
    }
}