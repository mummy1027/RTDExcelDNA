using System;
using MarketData.WCF;

namespace MarketData
{
    public interface IMarketDataProvider : IObservable<IMarketDataItem>, IDisposable
    {
        void Subscribe(string ric);
        void UnSubscribe(string ric);
    }
}