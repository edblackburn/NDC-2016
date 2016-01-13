using System;

namespace hacks.features.ip
{
    internal class Coin
    {
        private readonly Random _rnd = new Random();

        public int Flip()
        {
            var flip = _rnd.Next(1, 3)%2;
            return flip;
        }
    }
}