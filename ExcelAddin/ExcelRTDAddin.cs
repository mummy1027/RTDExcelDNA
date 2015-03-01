#region

using System;
using System.Reactive.Linq;
using System.Reflection;
using ExcelDna.Integration;
using MarketData;

#endregion

namespace ExcelAddin
{
    public class ExcelRTDAddin : IExcelAddIn
    {
        private static readonly IMarketDataProvider MarketDataProvider;

        static ExcelRTDAddin()
        {
            var config = new MarketDataConfig("net.tcp://localhost:8080/hello");
            MarketDataProvider = MarketDataProviderFactory.GetProvider(MarketDataProviderType.WCFClient, config);
        }

        public void AutoOpen()
        {
            ExcelIntegration.RegisterUnhandledExceptionHandler(ex => "!!! EXCEPTION: " + ex.ToString());

            object app = ExcelDnaUtil.Application;
            object rtd = app.GetType().InvokeMember("RTD", BindingFlags.GetProperty, null, app, null);
            rtd.GetType().InvokeMember("ThrottleInterval", BindingFlags.SetProperty, null, rtd, new object[] {100});
        }

        public void AutoClose()
        {
            MarketDataProvider.Dispose();
        }

        [ExcelFunction("Gets realtime values from server")]
        public static object GetValues(
            [ExcelArgument(Name = "Ric", Description = "Ric code for data subscription")] string ric)
        {
            MarketDataProvider.Subscribe(ric);

            Func<IObservable<double>> f2 = () => Observable.Create<double>(o => MarketDataProvider.Subscribe(i =>
            {
                if (i.Ric == ric)
                {
                    o.OnNext(i.Value);
                }
            }));

            //  pass that to Excel wrapper   
            return RxExcel.Observe("GetValues", ric, f2);
        }
    }
}