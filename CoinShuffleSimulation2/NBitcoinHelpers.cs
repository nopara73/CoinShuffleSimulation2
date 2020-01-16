using System;
using System.Collections.Generic;
using System.Text;

namespace NBitcoin
{
    public static class NBitcoinHelpers
    {
        public static bool IsScript(string s)
        {
            return s.Contains("OP_"); // This is stupid, but ok for the simulation.
        }
    }
}
