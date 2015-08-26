using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLIRC
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Channel: ");
            var chan = Console.ReadLine();
            var irc = new IRC("infallible.io", 6667, "ShebangBot", "ShebangBot", chan);
            irc.Connect();
        }
    }
}
