using DalSoft.Hosting.BackgroundQueue;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using SocialNetwork.Classes.Redis;
using SocialNetwork.Models;
using StackExchange.Redis;
using System.Collections.Concurrent;

namespace SocialNetwork.Classes.Services
{
    public class CacheService
    {
        public CacheService(RedisService redisService)
        {
            this.redisService = redisService;
        }

        private static readonly ConcurrentQueue<string> users = new ConcurrentQueue<string>();
        private readonly RedisService redisService;

        public RedisValue? GetPosts(string userID) {
            var key = getKey(userID);
            var value = redisService.GetResource(key);
            return value;
        }


        public void AddUserToUpdate(string userID)
        {
            users.Enqueue(userID);
        }

        public string GetUserForHandle()
        {
            users.TryDequeue(out var userID);
            return userID;
        }
     

        public void SavePostsToCache(string userID, List<PostModel> posts)
        {
            var key = getKey(userID);
            var value = JsonConvert.SerializeObject(posts);
            redisService.SetResource(key, value, Constants.UserCacheExpiry);
        }

        public void UpdatePostsTime(string userID) {
            var key = getKey(userID);
            redisService.SetExpire(key,Constants.UserCacheExpiry);

        }

        private string getKey(string userID) {
            var key = $"SocialNetwork:User:{userID}";
            return key ;
        }

    }
}
