using hacks.modelling.anemic;
using Moq;
using NUnit.Specifications;

namespace hacks.Testing
{
    internal class when_fare_is_assigned_to_journey : ContextSpecification
    {

        private Establish that = () =>
        {
            _mockery = new Mock<IFareRepository>();
            _service = new FareService(_mockery.Object);
            _bank = "Bank";
            _princeRegent = "Prince Regent";
            _journey = new Journey { Origin = _bank, Destination = _princeRegent };
        };

        private Because fare_assigned = () => _service.AssignFare(_journey);

        It should_invoke_reposoitory = () =>
            _mockery.Verify(r => r.GetFare(_bank, _princeRegent));

        private static Mock<IFareRepository> _mockery;
        private static FareService _service;
        private static Journey _journey;
        private static string _bank;
        private static string _princeRegent;
    }
}