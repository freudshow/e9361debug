using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E9361Debug.Config
{
    internal class Dlt645AddressConfig
    {
        public string Pattern;
    }

    internal class TerminalIdConfig
    {
        public string Pattern;
    }

    internal class Configuration
    {
        public Dlt645AddressConfig Dlt645Address;
        public TerminalIdConfig TerminalId;
    }
}