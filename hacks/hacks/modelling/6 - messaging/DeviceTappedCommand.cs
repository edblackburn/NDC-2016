using System;

namespace hacks.modelling.messaging
{
    public class DeviceTappedCommand
    {
        public DeviceTappedCommand(Guid accountId, string location, string mode)
        {
            AccountId = accountId;
            Location = location;
            Mode = mode;
            CorrelationId = SeqGuid.NewGuid();
        }

        public string Location { get; }
        public string Mode { get; }
        public Guid AccountId { get; }
        public Guid CorrelationId { get; set; }
    }
}