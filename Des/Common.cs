using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Des
{
    class Common
    {
        static public (byte[], byte[]) SplitBytes(byte[] input)
        {
            var up32 = input.Take(4);
            var lo32 = input.Skip(4);
            return (up32.ToArray(), lo32.ToArray());
        }
    }
}
