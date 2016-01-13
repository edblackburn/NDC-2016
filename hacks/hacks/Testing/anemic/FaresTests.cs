using hacks.modelling.anemic;
using Moq;
using NUnit.Framework;

namespace hacks.testing.anemic
{
    [TestFixture]
    public class FaresTests
    {
        [Test]
        public void BankToPrinceRegentFare()
        {
            var journey = new Journey {Origin = "Bank", Destination = "Prince Regent"};
            var service = new FareService(new FareRepository());

            service.AssignFare(journey);

            Assert.That(journey.Fare, Is.EqualTo(10));
        }

        [Test]
        public void BankToBankFare()
        {
            var bank = "Bank";
            var journey = new Journey {Origin = bank, Destination = bank};
            var repositoryMockery = new Mock<IFareRepository>();
            var fareService = new FareService(repositoryMockery.Object);

            fareService.AssignFare(journey);

            repositoryMockery.Verify(r => r.GetFare(bank, bank));
        }
    }

    internal class FareRepository : IFareRepository
    {
        public short GetFare(string origin, string destination)
        {
            return origin == "Bank" && destination == "Prince Regent" ? (short) 10 : (short) 0;
        }
    }
}