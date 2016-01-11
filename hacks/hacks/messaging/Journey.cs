using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace hacks.messaging
{
    internal class Journey : IEquatable<Journey>, IComparable<Journey>
    {
        private readonly List<Message> _messages = new List<Message>();
        private readonly Guid _id = SeqGuid.NewGuid();

        private short _fare;

        private readonly Func<List<Message>, DeviceTapped> _origin = messages =>
            messages.OfType<DeviceTapped>().FirstOrDefault();

        private readonly Func<List<Message>, DeviceTapped> _destination = messages =>
            messages.OfType<DeviceTapped>().LastOrDefault();

        internal void RecieveTap(DeviceTapped tap)
        {
            _messages.Add(tap);
            _messages.Sort(new MessageComparer());
        }

        internal void AssignFare(Fares fares)
        {
            var origin = _origin(_messages);
            if (null == origin)
            {
                _fare = 0;
                return;
            }

            var destination = _destination(_messages);
            if (null == destination)
            {
                _fare = 0;
                return;
            }
            _fare = fares(origin.Location, destination.Location);
        }

        internal dynamic Export()
        {
            var origin = string.Empty;
            var destination = string.Empty;

            var originTap = _origin(_messages);
            if (null != originTap)
            {
                origin = originTap.Location;
            }

            var destinationTap = _destination(_messages);
            if (null != destinationTap)
            {
                destination = destinationTap.Location;
            }

            return new
            {
                Origin = origin,
                Destination = destination,
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

    internal delegate short Fares(string origin, string destination);

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

            var msg1 = new DeviceTapped(bank);

            journey.RecieveTap(msg1);

            journey.AssignFare((o, d) => fare);

            Assert.That(journey.Export().Origin, Is.EqualTo(bank));
            Assert.That(journey.Export().Destination, Is.EqualTo(bank));
            Assert.That(journey.Export().Fare, Is.EqualTo(10));
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

            journey.AssignFare((o, d) => fare);

            Assert.That(journey.Export().Origin, Is.EqualTo(string.Empty));
            Assert.That(journey.Export().Destination, Is.EqualTo(string.Empty));
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

            var msg1 = new DeviceTapped(bank);
            var msg2 = new DeviceTapped(westernGateway);

            journey.RecieveTap(msg2);
            journey.RecieveTap(msg1);

            journey.AssignFare((o, d) => fare);

            Assert.That(journey.Export().Origin, Is.EqualTo(bank));
            Assert.That(journey.Export().Destination, Is.EqualTo(westernGateway));
            Assert.That(journey.Export().Fare, Is.EqualTo(10));
        }

        [Test]
        public void should_be_bank_to_bank()
        {
            const string bank = "Bank";
            const short fare = 10;

            var journey = new Journey();

            var msg1 = new DeviceTapped(bank);
            var msg2 = new DeviceTapped(bank);

            journey.RecieveTap(msg2);
            journey.RecieveTap(msg1);

            journey.AssignFare((o, d) => fare);

            Assert.That(journey.Export().Origin, Is.EqualTo(bank));
            Assert.That(journey.Export().Destination, Is.EqualTo(bank));
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

    internal class DeviceTapped : Message
    {
        public DeviceTapped(string location)
        {
            Location = location;
        }

        public string Location { get; }
    }
}