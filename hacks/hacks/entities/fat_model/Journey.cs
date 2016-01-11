using System;

namespace hacks.entities.fat_model
{
    internal class Journey
    {
        public Journey(string origin, string destination)
        {
            Origin = origin;
            Destination = destination;
        }

        public string Origin { get; }
        public string Destination { get; }
        public short Fare { get; private set; }

        internal void AssignFare(Func<string, string, short> fareQuery)
        {
            Fare = fareQuery(Origin, Destination);
        }
    }
}