using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using hacks.entities.value_objects;
using NUnit.Framework;

namespace hacks.messaging
{
    public class Journey : IEquatable<Journey>, IComparable<Journey>
    {
        private readonly List<Message> _messages = new List<Message>();
        private readonly Guid _id = SeqGuid.NewGuid();

        private short _fare;

        private readonly Func<List<Message>, DeviceTapped> _origin =
            messages => messages.OfType<DeviceTapped>().FirstOrDefault();

        private readonly Func<List<Message>, DeviceTapped> _destination =
            messages => messages.OfType<DeviceTapped>().LastOrDefault();

        public Journey():this(Guid.NewGuid())
        {
        }

        internal Journey(Guid accountId)
        {
            AccountId = accountId;
        }

        public Guid AccountId { get; }

        internal void RecieveTap(DeviceTapped tap)
        {
            _messages.Add(tap);
            _messages.Sort(new MessageComparer());
        }

        internal void AssignFare(Fares fares)
        {
            var originDestination = GetOriginDestination();
            if (OriginDestination.HasNoJourney(originDestination)) return;
            _fare = fares(originDestination);
        }

        private OriginDestination GetOriginDestination()
        {
            var origin = _origin(_messages);
            var destination = _destination(_messages) ?? origin;

            if (null == origin && null == destination) return OriginDestination.NoJourney();

            var originDestination = OriginDestination.OriginToDestination(origin.Location, destination.Location);

            return originDestination;
        }

        internal dynamic Export()
        {
            return new
            {
                OriginDestination = GetOriginDestination(),
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

    [TestFixture]
    public class when_journey_recieves_here_to_here_tap_message
    {
        [Test]
        public void should_be_bank_to_bank()
        {
            const string bank = "Bank";
            const short fare = 10;

            var journey = new Journey();

            var msg1 = new DeviceTapped(journey.AccountId, bank, "rail");

            journey.RecieveTap(msg1);

            journey.AssignFare(od => fare);

            Func<dynamic> export = journey.Export;

            Assert.That(export().OriginDestination, Is.EqualTo(OriginDestination.HereToHere(bank)));
            Assert.That(export().Fare, Is.EqualTo(10));
        }
    }

    [TestFixture]
    public class when_journey_recieves_no_tap
    {
        [Test]
        public void should_have_no_fare()
        {
            const short fare = 10;

            var journey = new Journey();

            journey.AssignFare(od => fare);

            Assert.That(OriginDestination.HasNoJourney(journey.Export().OriginDestination), Is.True);
            Assert.That(journey.Export().Fare, Is.EqualTo(0));
        }
    }

    [TestFixture]
    public class when_journey_recieves_origin_to_destination_tap_message
    {
        [Test]
        public void should_be_bank_to_western_gate()
        {
            const string bank = "Bank";
            const string westernGateway = "Western Gateway";
            const short fare = 10;

            var journey = new Journey();

            var msg1 = new DeviceTapped(journey.AccountId, bank, "rail");
            var msg2 = new DeviceTapped(journey.AccountId, westernGateway, "rail");

            journey.RecieveTap(msg2);
            journey.RecieveTap(msg1);

            journey.AssignFare(od => fare);

            Assert.That(journey.Export().OriginDestination,
                Is.EqualTo(OriginDestination.OriginToDestination(bank, westernGateway)));
            Assert.That(journey.Export().Fare, Is.EqualTo(10));
        }

        [Test]
        public void should_be_bank_to_bank()
        {
            const string bank = "Bank";
            const short fare = 10;

            var journey = new Journey();

            var msg1 = new DeviceTapped(journey.AccountId, bank, "rail");
            var msg2 = new DeviceTapped(journey.AccountId, bank, "rail");

            journey.RecieveTap(msg2);
            journey.RecieveTap(msg1);

            journey.AssignFare(od => fare);

            Assert.That(journey.Export().OriginDestination, Is.EqualTo(OriginDestination.HereToHere(bank)));
            Assert.That(journey.Export().Fare, Is.EqualTo(10));
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

    public class DeviceTapped : Message
    {
        public DeviceTapped(Guid accountId, string location, string mode)
        {
            AccountId = accountId;
            Location = location;
            Mode = mode;
        }

        public string Location { get; }
        public string Mode { get; }
        public Guid AccountId { get; }
    }
}