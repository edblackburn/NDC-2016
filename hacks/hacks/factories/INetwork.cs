using hacks.modelling.value_objects;

namespace hacks.factories
{
    public interface INetwork
    {
        short GetFare(OriginDestination originDestination, string mode);
    }
}