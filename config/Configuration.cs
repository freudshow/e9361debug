namespace E9361Debug.Config
{
    internal class Dlt645AddressConfig
    {
        public string Pattern = "";
    }

    internal class TerminalIdConfig
    {
        public string Pattern = "";
    }

    internal class Configuration
    {
        public Dlt645AddressConfig Dlt645Address = new Dlt645AddressConfig();
        public TerminalIdConfig TerminalId = new TerminalIdConfig();
    }
}