using System;

namespace hacks.modelling.anemic
{
    internal class Journey
    {
        public Guid JouneyId { get; set; }
        public Guid AccountId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public short Fare { get; set; }
    }

    internal class FareService
    {
        private readonly IFareRepository _fareFareRepository;

        public FareService(IFareRepository fareFareRepository)
        {
            _fareFareRepository = fareFareRepository;
        }

        internal void AssignFare(Journey jny)
        {
            jny.Fare = _fareFareRepository.GetFare(jny.Origin, jny.Destination);
        }
    }

    public interface IFareRepository
    {
        short GetFare(string origin, string destination);
    }

}