using System;
using System.IO;

namespace hacks.abstraction
{
    internal abstract class Animal
    {
        protected internal TextWriter Writer { get; }

        protected Animal(TextWriter writer)
        {
            Writer = writer;
        }

        internal abstract void Sound();

        internal abstract short NumberOfLegs { get; }

        internal virtual void Move()
        {
            for (var leg = 0; leg < NumberOfLegs; leg++)
            {
                Writer.WriteLine("Moved leg {0}", leg);
            }
        }
    }

    internal class Lion : Animal
    {
        public Lion() : base(Console.Out)
        {
            NumberOfLegs = 4;
        }

        internal override void Sound()
        {
            Writer.WriteLine("Roar");
        }

        internal override short NumberOfLegs { get; }
    }

    internal class Snake : Animal
    {
        public Snake() : base(Console.Out)
        {
        }

        internal override void Sound()
        {
            Writer.WriteLine("Hiss");
        }

        internal override short NumberOfLegs { get; }

        internal override void Move()
        {
            Writer.WriteLine("Slither");
        }
    }
}