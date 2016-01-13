using System;

namespace hacks.modelling.tell_dont_ask
{
    internal delegate short Fares(string origin, string destination);

    internal class Journey
    {
        private readonly Guid _id;
        private readonly Guid _accountId;
        private readonly string _origin;
        private readonly string _destination;
        private short _fare;

        public Journey(Guid id, Guid accountId, string origin, string destination)
        {
            _id = id;
            _accountId = accountId;
            _origin = origin;
            _destination = destination;
        }

        internal void AssignFare(Fares fares)
        {
            _fare = fares(_origin, _destination);
        }

        internal dynamic Project()
        {
            return new
            {
                AccountId = _accountId,
                JourneyId = _id,
                Origin = _origin,
                Destination = _destination,
                Fare = _fare
            };
        }
    }
}