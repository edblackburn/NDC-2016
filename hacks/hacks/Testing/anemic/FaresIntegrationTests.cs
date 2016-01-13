using hacks.modelling.anemic;
using NUnit.Framework;

namespace hacks.testing.anemic
{
    [TestFixture]
    public class FaresIntegrationTests
    {
        [Test]
        public void BankToPrinceRegentFare()
        {
            var journey = new Journey {Origin = "Bank", Destination = "Prince Regent"};
            var fareService = new FareService(new SqlFareRepository());

            fareService.AssignFare(journey);

            Assert.That(journey.Fare, Is.EqualTo(10));
        }

        internal class SqlFareRepository : IFareRepository
        {
            public short GetFare(string origin, string destination)
            {
                return origin == "Bank" && destination == "Prince Regent" ? (short)10 : (short)0;
            }
        }

    }
}