using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace hacks.inversion_of_control
{
    internal class Journey : IEquatable<Journey>, IComparable<Journey>
    {
        private readonly List<Message> _messages = new List<Message>();
        private readonly Guid _id = SeqGuid.NewGuid();

        private short _fare;
        private OriginDestination _originDestination = OriginDestination.Transient();
        private string _mode;

        internal void RecieveTap(DeviceTapped tap)
        {
            _messages.Add(tap);
            _messages.Sort(new MessageComparer());
            var originTap = _messages.OfType<DeviceTapped>().FirstOrDefault();
            var destinationTap = _messages.OfType<DeviceTapped>().LastOrDefault();
            if (null != originTap && null != destinationTap)
            {
                _originDestination = OriginDestination.OriginToDestination(originTap.Location, destinationTap.Location);
            }
            if (_messages.OfType<DeviceTapped>().All(t => t.Mode != tap.Mode))
            {
                throw new InvalidOperationException("Journey cannot span multiple modes");
            }

            _mode = tap.Mode;
        }

        internal void AssignFare(Fares fares)
        {
            _fare = OriginDestination.IsTransient(_originDestination)
                ? (short) 0
                : fares(_originDestination, _mode);
        }

        internal dynamic Export()
        {
            return new
            {
                _originDestination.Origin,
                _originDestination.Destination,
                Fare = _fare
            };
        }

        public bool Equals(Journey other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return _id.Equals(other._id);
        }

        public int CompareTo(Journey other)
        {
            return _id.CompareTo(other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Journey)) return false;
            return Equals((Journey) obj);
        }

        public override int GetHashCode()
        {
            return _id.GetHashCode();
        }
    }

    internal struct OriginDestination : IEquatable<OriginDestination>
    {
        public readonly string Origin;
        public readonly string Destination;

        public static OriginDestination Transient()
        {
            return new OriginDestination();
        }

        public static OriginDestination HereToHere(string location)
        {
            return new OriginDestination(location, location);
        }

        public static OriginDestination OriginToDestination(string origin, string destination)
        {
            return new OriginDestination(origin, destination);
        }

        public static bool IsTransient(OriginDestination originDestination)
        {
            return string.Empty == originDestination.Origin
                   && string.Empty == originDestination.Destination;
        }

        private OriginDestination(string origin, string destination)
        {
            if (string.IsNullOrWhiteSpace(origin))
            {
                const string originName = nameof(origin);
                throw new ArgumentException($"{originName} cannot be null or empty", originName);
            }

            if (string.IsNullOrWhiteSpace(destination))
            {
                const string destinationName = nameof(destination);
                throw new ArgumentException($"{destinationName} cannot be null or empty", destinationName);
            }

            Origin = origin;
            Destination = destination;
        }

        public bool Equals(OriginDestination other)
        {
            return string.Equals(Origin, other.Origin)
                   && string.Equals(Destination, other.Destination);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is OriginDestination && Equals((OriginDestination) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Origin.GetHashCode()*397) ^ Destination.GetHashCode();
            }
        }
    }

    internal delegate short Fares(OriginDestination originDestination, string mode);

    internal class SwitchNetwork
    {
        internal short GetFare(OriginDestination originDestination, string mode)
        {
            switch (mode)
            {
                case "rail":
                    return 10;
                case "bus":
                    return 2;
                default:
                    throw new InvalidOperationException($"Cannot get fare for mode {mode}");
            }
        }
    }

    internal class MappedNetwork
    {
        internal short GetFare(OriginDestination originDestination, string mode)
        {
            var map = new Dictionary<string, Func<OriginDestination, short>>
            {
                {"rail", od => 10},
                {"bus", od => 10}
            };

            if (map.ContainsKey(mode))
            {
                return map[mode](originDestination);
            }

            throw new InvalidOperationException($"Cannot get fare for mode {mode}");
        }
    }

    internal class Bus : INetwork, ISatisfyNetwork
    {
        public short GetFare(OriginDestination originDestination, string mode)
        {
            return 2;
        }

        public bool Matches(string mode)
        {
            return string.Equals("bus", mode, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    internal class Rail : INetwork, ISatisfyNetwork
    {
        public short GetFare(OriginDestination originDestination, string mode)
        {
            return 2;
        }

        public bool Matches(string mode)
        {
            return string.Equals("rail", mode, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    internal class InjectedNetwork : INetwork
    {
        private readonly INetwork[] _networks;

        public InjectedNetwork(INetwork[] networks)
        {
            _networks = networks;
        }

        public short GetFare(OriginDestination originDestination, string mode)
        {
            foreach (var network in _networks.OfType<ISatisfyNetwork>())
            {
                return ((INetwork) network).GetFare(originDestination, mode);
            }

            throw new InvalidOperationException($"Cannot get fare for mode {mode}");
        }
    }

    internal interface ISatisfyNetwork
    {
        bool Matches(string mode);
    }

    internal interface INetwork
    {
        short GetFare(OriginDestination originDestination, string mode);
    }

    internal class SeqGuid
    {
        [DllImport("rpcrt4.dll", SetLastError = true)]
        private static extern int UuidCreateSequential(out Guid guid);

        public static Guid NewGuid()
        {
            Guid guid;
            return UuidCreateSequential(out guid) == 0 ? guid : Guid.NewGuid();
        }
    }

    public class Message
    {
        private Message(Guid messageId)
        {
            MessageId = messageId;
        }

        public Message() : this(SeqGuid.NewGuid())
        {
        }

        public Guid MessageId { get; }
        public Guid CorrelationId { get; set; }
    }

    public sealed class MessageComparer : IComparer<Message>
    {
        public int Compare(Message x, Message y)
        {
            return x.MessageId.CompareTo(y.MessageId);
        }
    }

    internal class DeviceTapped : Message
    {
        public DeviceTapped(string location, string mode)
        {
            Location = location;
            Mode = mode;
        }

        public string Location { get; }
        public string Mode { get; }
    }

    [TestFixture]
    public class when_journey_recieves_here_to_here_tap_message
    {
        [Test]
        public void should_be_bank_to_bank()
        {
            const string dlr = "DLR";
            const string bank = "Bank";
            const short fare = 10;

            var journey = new Journey();

            var msg1 = new DeviceTapped(bank, dlr);

            journey.RecieveTap(msg1);

            journey.AssignFare((od, m) => fare);

            Assert.That(journey.Export().Origin, Is.EqualTo(bank));
            Assert.That(journey.Export().Destination, Is.EqualTo(bank));
            Assert.That(journey.Export().Fare, Is.EqualTo(10));
        }
    }
}