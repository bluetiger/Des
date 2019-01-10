using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Des
{
    class Encrypt
    {
        static public byte[] Run(byte[] input)
        {
            var up32 = input.Take(32);
            var lo32 = input.Skip(32);
            return null;
        }
    }
}
