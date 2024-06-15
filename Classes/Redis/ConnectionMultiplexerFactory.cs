namespace SocialNetwork.Classes.Redis
{
    using System.Threading.Tasks;
    using StackExchange.Redis;

    public class ConnectionMultiplexerCreator
    {
        private readonly string _redisServer;
        private readonly int _redisPort;
        private readonly string _connectionString;
        private readonly string _redisTlsCert;
        private readonly string _redisTlsKey;


        public ConnectionMultiplexerCreator(string redisServer, int redisPort, string redisTlsCert, string redisTlsKey)
        {
            _redisServer = redisServer;
            _redisPort = redisPort;
            _redisTlsCert = redisTlsCert;
            _redisTlsKey = redisTlsKey;
        }

        public ConnectionMultiplexerCreator(string connectionString, string redisTlsCert, string redisTlsKey)
        {
            _connectionString = connectionString;
            _redisTlsCert = redisTlsCert;
            _redisTlsKey = redisTlsKey;
        }

        public  IConnectionMultiplexer Create()
        {
            var options = getOptions();
            return ConnectionMultiplexer.Connect(options);
        }


        public async Task<IConnectionMultiplexer> CreateAsync()
        {

            var options = getOptions();
            return await ConnectionMultiplexer.ConnectAsync(options);           
        }

        public string Server => _redisServer;
        public int Port => _redisPort;

        private ConfigurationOptions getOptions()
        {
            ConfigurationOptions options;
            if (!string.IsNullOrEmpty(_connectionString))
            {

                options = ConfigurationOptions.Parse(_connectionString);
            }
            else
            {
                options = new ConfigurationOptions
                {
                    EndPoints = { { _redisServer, _redisPort } },
                    AllowAdmin = true
                };
            }
            options.ReconnectRetryPolicy = new ExponentialRetry(500, 1000);
            options.SyncTimeout = 10000;

            options.CertificateSelection += delegate {
                return new System.Security.Cryptography.X509Certificates.X509Certificate2(_redisTlsCert, _redisTlsKey);
            };

            return options;
        }
    }
}
