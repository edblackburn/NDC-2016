using System;
using hacks.modelling.messaging;

namespace hacks.factories
{
    public interface IAccount
    {
        Journey Get(Guid id);
        void Store(Journey journey);
    }
}