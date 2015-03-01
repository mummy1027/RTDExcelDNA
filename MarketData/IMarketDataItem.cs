namespace MarketData
{
    public interface IMarketDataItem
    {
        string Ric { get; }
        double Value { get; }
    }
}