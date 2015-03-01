using System;
using System.Reactive;
using log4net.Config;
using MarketData;
using MarketData.WCF;

namespace SampleAppClient
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            var config = new MarketDataConfig("net.tcp://localhost:8080/hello");

            var marketDataProvider = MarketDataProviderFactory.GetProvider(MarketDataProviderType.WCFClient, config);

            var observerA = Observer.Create<IMarketDataItem>(o => Console.WriteLine("Update {0} {1}", o.Ric, o.Value));

            marketDataProvider.Subscribe(observerA);

            marketDataProvider.Subscribe("0005.HK");
            marketDataProvider.Subscribe("VOD.L");
            marketDataProvider.Subscribe("VOD.L");
            marketDataProvider.Subscribe(".FTSE");

            Console.WriteLine("Client is up, press <Return> to close");
            Console.ReadLine();
            marketDataProvider.Dispose();
        }
    }

}
