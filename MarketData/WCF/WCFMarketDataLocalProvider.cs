using System;
using System.Collections.Concurrent;
using System.Reactive.Subjects;

namespace MarketData.WCF
{
    internal class WCFMarketDataLocalProvider : IMarketDataProvider
    {
        private readonly IMarketDataProvider _remoteServer;
        private readonly ConcurrentDictionary<string, IDisposable> _subscriptions = new ConcurrentDictionary<string, IDisposable>();

        public WCFMarketDataLocalProvider(IMarketDataProvider remoteServer)
        {
            _remoteServer = remoteServer;
        }

        public void Dispose()
        {
            _remoteServer.Dispose();
        }

        public void Subscribe(string ric)
        {
            if (!_subscriptions.ContainsKey(ric))
            {
                _remoteServer.Subscribe(ric);
                _subscriptions.TryAdd(ric, null);
            }
        }

        public void UnSubscribe(string ric)
        {
            IDisposable disposable = null;
            if (_subscriptions.TryRemove(ric, out disposable))
            {
                _remoteServer.UnSubscribe(ric);
            }
        }

        public IDisposable Subscribe(IObserver<IMarketDataItem> observer)
        {
            return _remoteServer.Subscribe(observer);
        }
    }
}