using MarketData.WCF;

namespace MarketData
{
    public class MarketDataConfig : IMarketDataConfig
    {
        private readonly string _wcfHostAddress;

        public MarketDataConfig(string wcfHostAdddress)
        {
            _wcfHostAddress = wcfHostAdddress;
        }

        public string WCFHostAddress
        {
            get { return _wcfHostAddress; }
        }
    }
}