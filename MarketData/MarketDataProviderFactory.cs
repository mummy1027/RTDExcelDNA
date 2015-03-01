using System;
using MarketData.WCF;

namespace MarketData
{
    public enum MarketDataProviderType
    {
        WCFLocalServer,
        WCFRemoteSever,
        WCFClient,
        NetMQServer,
        NetMQClient
    }

    public class MarketDataProviderFactory
    {
        public static IMarketDataProvider GetProvider(MarketDataProviderType type, IMarketDataConfig config)    
        {
            switch (type)
            {
                case MarketDataProviderType.WCFClient:
                    return new WCFMarketDataLocalProvider(GetProvider(MarketDataProviderType.WCFRemoteSever, config));
                case MarketDataProviderType.WCFLocalServer:
                    return new WCFMarketDataLocalServerProvider();
                case MarketDataProviderType.WCFRemoteSever:
                    return new WCFMarketDataRemoteServerProvider(config, new MarketDataCallbackChannel());
                case MarketDataProviderType.NetMQServer:
                case MarketDataProviderType.NetMQClient:
                    return null;
                default :
                    throw new Exception("Unsupported type");
            }
        }
    }
}