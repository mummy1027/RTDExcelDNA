#region

using System;
using System.Reactive.Subjects;
using log4net;

#endregion

namespace MarketData.WCF
{
    internal class MarketDataCallbackChannel : IMarketDataCallbackChannel
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (MarketDataCallbackChannel));
        private readonly ISubject<IMarketDataItem> _subject = new Subject<IMarketDataItem>();

        public void UpdateData(IMarketDataItem marketDataItem)
        {
            try
            {
                _subject.OnNext(marketDataItem);
            }
            catch (Exception exception)
            {
                Logger.ErrorFormat("Error when update data {0} : {1}", marketDataItem, exception);
            }
        }

        public IDisposable Subscribe(IObserver<IMarketDataItem> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}