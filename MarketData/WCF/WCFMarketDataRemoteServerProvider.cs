#region

using System;
using System.Reactive.Subjects;
using System.ServiceModel;

#endregion

namespace MarketData.WCF
{
    public class WCFMarketDataRemoteServerProvider : IMarketDataProvider
    {
        private readonly IWCFMarketDataProvider _remoMarketDataProvider;
        private readonly ISubject<IMarketDataItem> _subject = new Subject<IMarketDataItem>();
        private readonly IDisposable _subscription;

        public WCFMarketDataRemoteServerProvider(IMarketDataConfig config, IMarketDataCallbackChannel callbackChannel)
        {
            _remoMarketDataProvider = DuplexChannelFactory<IWCFMarketDataProvider>.CreateChannel(
                new InstanceContext(callbackChannel),
                new NetTcpBinding(),
                new EndpointAddress(config.WCFHostAddress));
            _subscription = callbackChannel.Subscribe(_subject);
        }

        public void Dispose()
        {
            if (_subscription != null)
            {
                _subscription.Dispose();
            }
        }

        public void Subscribe(string ric)
        {
            _remoMarketDataProvider.Subscribe(ric);
        }

        public void UnSubscribe(string ric)
        {
            _remoMarketDataProvider.UnSubscribe(ric);
        }

        public IDisposable Subscribe(IObserver<IMarketDataItem> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}