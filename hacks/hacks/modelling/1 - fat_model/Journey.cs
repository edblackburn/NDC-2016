using System;

namespace hacks.modelling.fat_model
{
    internal class Journey
    {
        public Journey(Guid id, Guid accountId, string origin, string destination)
        {
            JourneyId = id;
            AccountId = accountId;
            Origin = origin;
            Destination = destination;
        }

        public Guid JourneyId { get; }
        public Guid AccountId { get; }
        public string Origin { get; }
        public string Destination { get; }
        public short Fare { get; private set; }

        internal void AssignFare(Func<string, string, short> fareQuery)
        {
            Fare = fareQuery(Origin, Destination);
        }
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
            jny.AssignFare((o, d) => _fareFareRepository.GetFare(o, d));
        }
    }

    internal interface IFareRepository
    {
        short GetFare(string origin, string destination);
    }
}