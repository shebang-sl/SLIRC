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
            var irc = new IRC("infallible.io", 6667, "ShebangBot", "ShebangBot", "#SL");
            irc.Connect();
        }
    }
}
