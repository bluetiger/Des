using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Des
{
    class Program
    {
        static void Main(string[] args)
        {
            var target = "abcdefgh";
            var bytes = System.Text.Encoding.UTF8.GetBytes(target);
            var sp = Common.SplitBytes(bytes);
            foreach (var v in sp.Item1)
            {
                Console.WriteLine(Convert.ToString(v,2).PadLeft(8, '0'));
            }
            
        }
    }
}
