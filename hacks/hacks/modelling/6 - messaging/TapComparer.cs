using System.Collections.Generic;

namespace hacks.modelling.messaging
{
    public sealed class TapComparer : IComparer<DeviceTappedCommand>
    {
        public int Compare(DeviceTappedCommand leftTap, DeviceTappedCommand rightTap)
        {
            return leftTap.CorrelationId.CompareTo(rightTap.CorrelationId);
        }
    }
}