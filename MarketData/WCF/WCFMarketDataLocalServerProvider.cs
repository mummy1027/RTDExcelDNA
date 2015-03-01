#region

using System;
using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Subjects;
using System.ServiceModel;
using System.Timers;
using log4net;

#endregion

namespace MarketData.WCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    internal class WCFMarketDataLocalServerProvider : IWCFMarketDataProvider, IMarketDataProvider
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (WCFMarketDataLocalServerProvider));

        private readonly ConcurrentDictionary<IMarketDataCallbackChannel, ConcurrentDictionary<string, IDisposable>> _clients;

        private readonly string[] _rics = {"0005.HK", ".FTSE", "VOD.L", ".N225"};

        private readonly Random _rnd;
        private readonly Subject<IMarketDataItem> _subject = new Subject<IMarketDataItem>();

        public WCFMarketDataLocalServerProvider()
        {
            _clients = new ConcurrentDictionary<IMarketDataCallbackChannel, ConcurrentDictionary<string, IDisposable>>();
            _rnd = new Random();
            var timer = new Timer(500);
            timer.Elapsed += SendData;
            timer.Start();
        }

        public IDisposable Subscribe(IObserver<IMarketDataItem> observer)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            foreach (var value in _clients.Values)
            {
                foreach (var disposbal in value.Values)
                {
                    disposbal.Dispose();
                }
            }
            _clients.Clear();
        }

        public void Subscribe(string ric)
        {
            var c = OperationContext.Current.GetCallbackChannel<IMarketDataCallbackChannel>();
            _clients.AddOrUpdate(c, delegate
            {
                var o = _subject.Subscribe(Observer.Create<IMarketDataItem>(i =>
                {
                    try
                    {
                        if (i.Ric == ric)
                        {
                            c.UpdateData(i);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat("Error when OnNext {0}", ex);
                    }
                }));
                var d = new ConcurrentDictionary<string, IDisposable>();
                d.TryAdd(ric, o);
                return d;
            }, (k, v) =>
            {
                var o = _subject.Subscribe(Observer.Create<IMarketDataItem>(i =>
                {
                    try
                    {
                        if (i.Ric == ric)
                        {
                            c.UpdateData(i);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.ErrorFormat("Error when OnNext {0}", ex);
                    }
                }));
                if (!v.TryAdd(ric, o))
                {
                    o.Dispose();
                }
                return v;
            });
        }

        public void UnSubscribe(string ric)
        {
            var c = OperationContext.Current.GetCallbackChannel<IMarketDataCallbackChannel>();
            ConcurrentDictionary<string, IDisposable> subscriptions;
            if (_clients.TryGetValue(c, out subscriptions))
            {
                IDisposable disposable;
                if (subscriptions.TryRemove(ric, out disposable))
                {
                    disposable.Dispose();
                }

                if (subscriptions.IsEmpty)
                {
                    _clients.TryRemove(c, out subscriptions);
                }
            }
        }

        private void SendData(object sender, ElapsedEventArgs e)
        {
            var next = _rnd.NextDouble();
            var ric = GetRicCode();
            Console.WriteLine("Sending {0} {1}", ric, next);

            _subject.OnNext(new SimpleMarketDataItem {Value = next, Ric = ric});
        }

        public void UnRegister(IMarketDataCallbackChannel c)
        {
            ConcurrentDictionary<string,IDisposable> disposables;
            _clients.TryRemove(c, out disposables);
            if (disposables != null)
            {
                foreach (var disposable in disposables)
                {
                    disposable.Value.Dispose();
                }
            }
        }

        private string GetRicCode()
        {
            var next = _rnd.Next()%4;
            return _rics[next];
        }
    }
}