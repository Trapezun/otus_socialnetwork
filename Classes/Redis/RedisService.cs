using StackExchange.Redis;

namespace SocialNetwork.Classes.Redis
{
    public class RedisService
    {
        public RedisService(RedisWrapper redisWrapper)
        {
            _redisWrapper= redisWrapper;
        }

        private RedisWrapper _redisWrapper;                        
        public RedisValue? GetResource(string key)
        {                   
            var redisClient = _redisWrapper;
            var value = redisClient.GetString(1, key);
            if (value == null)
            {
                return null;
            }
            if (value.Value.IsNull)
            {               
                return null;
            }          
            return value;
        }


        public void SetResource(string key, string value, TimeSpan? expiry = null)
        {
            var redisClient = _redisWrapper;
            redisClient.SetString(1, key, value, expiry);
        }

        public void SetExpire(string key, TimeSpan? expiry = null)
        {
            var redisClient = _redisWrapper;
            redisClient.SetExpire(1, key,expiry);
        }
    }
}
