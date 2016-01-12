using System;
using System.Collections.Generic;
using hacks.modelling.messaging;

namespace hacks.factories.specs
{
    internal class AccountSpy : IAccount
    {
        private readonly IDictionary<Guid, Journey> _journeys = new Dictionary<Guid, Journey>();

        public Journey Get(Guid id)
        {
            return _journeys.ContainsKey(id) ? _journeys[id] : new Journey(id);
        }

        public void Store(Journey journey)
        {
            _journeys[journey.AccountId] = journey;
        }
    }
}