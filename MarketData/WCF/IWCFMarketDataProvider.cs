using System.ServiceModel;

namespace MarketData.WCF
{
    [ServiceContract(CallbackContract = typeof(IMarketDataCallbackChannel))]
    public interface IWCFMarketDataProvider
    {
        [OperationContract(IsOneWay = true)]
        void Subscribe(string ric);

        [OperationContract(IsOneWay = true)]
        void UnSubscribe(string ric);
    }
}