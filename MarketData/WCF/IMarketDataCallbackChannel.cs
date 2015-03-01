#region

using System;
using System.ServiceModel;

#endregion

namespace MarketData.WCF
{
    [ServiceKnownType(typeof (SimpleMarketDataItem))]
    public interface IMarketDataCallbackChannel : IObservable<IMarketDataItem>
    {
        [OperationContract(IsOneWay = true)]
        void UpdateData(IMarketDataItem marketDataItem);
    }
}