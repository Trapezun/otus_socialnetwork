using System;

namespace SocialNetwork.Classes.Redis
{
    using StackExchange.Redis;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;


    public class RedisWrapper : IDisposable
    {
        bool disposed = false;

        private IConnectionMultiplexer _redisMultiplexer;
        private readonly ConnectionMultiplexerCreator _connectionMultiplexerFactory;
        private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

        public RedisWrapper(ConnectionMultiplexerCreator connectionMultiplexerFactory)
        {
            _connectionMultiplexerFactory = connectionMultiplexerFactory;
        }

        public void InitMultiplexer()
        {
            if (_redisMultiplexer != null)
            {
                return;
            }

            _connectionLock.Wait();

            try
            {
                if (_redisMultiplexer == null)
                {
                    _redisMultiplexer = _connectionMultiplexerFactory.Create();
                }
            }
            finally
            {
                _connectionLock.Release();
            }
        }

        public RedisValue? GetString(int redisDbNumber, string key)
        {
            InitMultiplexer();
            var db = GetDatabase(redisDbNumber);
            var value = getWithRetry(db, key);
            return value;
        }

        public List<string> GetStrings(int redisDbNumber, List<RedisKey> keys)
        {
            InitMultiplexer();
            var db = GetDatabase(redisDbNumber);
            return GetStrings(redisDbNumber, keys.ToArray());
        }

        public List<string> GetStrings(int redisDbNumber, RedisKey[] keys)
        {
            InitMultiplexer();
            var db = GetDatabase(redisDbNumber);

            var values = getWithRetry(db, keys);
            if (values == null)
            {
                return null;
            }

            var retval = Array.ConvertAll(values, x => (string)x).ToList();
            return retval;
        }


        public IConnectionMultiplexer GetMultiplexer()
        {
            InitMultiplexer();
            return _redisMultiplexer;
        }



        public void SetString(int redisDbNumber, string key, RedisValue value, TimeSpan? expiry = null)
        {
            InitMultiplexer();
            var db = GetDatabase(redisDbNumber);
            db.StringSet(key, value, expiry);
        }

        public void DeleteKey(int redisDbNumber, string key)
        {
            InitMultiplexer();
            var db = GetDatabase(redisDbNumber);
            db.KeyDelete(key);
        }

        public IServer GetServer()
        {
            InitMultiplexer();
            var url = _redisMultiplexer.Configuration.Split(',').ToList().First();
            return _redisMultiplexer.GetServer(url);
        }



        public void Publish(string channel, string message)
        {
            InitMultiplexer();
            if (_redisMultiplexer != null)
            {
                ISubscriber sub = _redisMultiplexer.GetSubscriber();
                sub.Publish(channel, message);
            }
        }

        public void Subscribe(string channel, Action<string, string> handler)
        {
            InitMultiplexer();
            if (_redisMultiplexer != null)
            {
                var sub = _redisMultiplexer.GetSubscriber();
                var action = new Action<RedisChannel, RedisValue>((_channel, _value) =>
                {
                    handler.Invoke(_channel, _value);
                });
                sub.Subscribe(channel, action);
            }
        }



        public void Dispose()
        {
            dispose(true);
        }

        protected virtual void dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (_redisMultiplexer != null)
                {
                    _redisMultiplexer.Close();
                }
            }

            disposed = true;
        }


        public IDatabase GetDatabase(int redisDbNumber)
        {
            InitMultiplexer();
            return _redisMultiplexer.GetDatabase(redisDbNumber);
        }


        private RedisValue? getWithRetry(IDatabase db, string key)
        {
            int wait = 10;
            int retryCount = 3;

            int i = 0;
            do
            {
                try
                {
                    return db.StringGet(key);
                }
                catch (Exception)
                {
                    if (i < retryCount + 1)
                    {
                        Thread.Sleep(wait);
                        i++;
                    }
                    else throw;
                }
            }
            while (i < retryCount + 1);
            return null;
        }

        private RedisValue[] getWithRetry(IDatabase db, RedisKey[] keys)
        {
            int wait = 10;
            int retryCount = 3;

            int i = 0;
            do
            {
                try
                {
                    return db.StringGet(keys);
                }
                catch (Exception)
                {
                    if (i < retryCount + 1)
                    {
                        Thread.Sleep(wait);
                        i++;
                    }
                    else throw;
                }
            }
            while (i < retryCount + 1);
            return null;
        }

    }
}
