using System;
using System.Runtime.InteropServices;

namespace hacks
{
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
}