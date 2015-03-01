using System.Runtime.Serialization;

namespace MarketData.WCF
{
    [DataContract]
    internal class SimpleMarketDataItem : IMarketDataItem
    {
        [DataMember]
        public string Ric { get; set; }

        [DataMember]
        public double Value { get; set; }
    }
}