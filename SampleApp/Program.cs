#region

using System;
using System.ServiceModel;
using log4net.Config;
using MarketData;
using MarketData.WCF;

#endregion

namespace SampleApp
{
    class Program
    {
        private static void Main(string[] args)
        {
            XmlConfigurator.Configure();

            Uri baseAddress = new Uri("net.tcp://localhost:8080/hello");
            var marketDataProvider = MarketDataProviderFactory.GetProvider(MarketDataProviderType.WCFLocalServer, null);
            var host = new ServiceHost(marketDataProvider, baseAddress);
            host.Open();

            Console.WriteLine("The service is ready at {0}", baseAddress);
            Console.WriteLine("Press <Enter> to stop the service.");
            Console.ReadLine();

            host.Close();
        }
    }
}