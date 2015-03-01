using System;
using System.ServiceModel;

namespace MarketData.WCF
{
    [ServiceKnownType(typeof(SimpleMarketDataItem))]
    public interface IWCFMarketDataRemoteClient : IObservable<IMarketDataItem>
    {
        [OperationContract(IsOneWay = true)]
        void OnNext(IMarketDataItem x);
    }
}