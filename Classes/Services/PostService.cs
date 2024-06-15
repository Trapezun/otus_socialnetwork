using DalSoft.Hosting.BackgroundQueue;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SocialNetwork.Classes.Redis;
using SocialNetwork.Models;
using System.Text.Json.Serialization;

namespace SocialNetwork.Classes.Services
{
    public class PostService
    {
        private readonly ApplicationContext applicationContext;
        private readonly RedisService redisService;
        private readonly BackgroundQueue backgroundQueue;

        public PostService(ApplicationContext applicationContext, RedisService redisService, BackgroundQueue backgroundQueue)
        {
            this.applicationContext = applicationContext;
            this.redisService = redisService;
            this.backgroundQueue = backgroundQueue;
        }



        public List<PostModel> Feed(string myUserID, int number, int limit)
        {
            var key = $"SocialNetwork:User:{myUserID}";
            var value = redisService.GetResource(key);
            if (value != null)
            {
                var retval = JsonConvert.DeserializeObject<List<PostModel>>(value);
                retval = retval.Skip(number).Take(limit).ToList();
                return retval;
            }
            else
            {
                var posts = applicationContext.Friendships
                    .Where(f => f.userID == myUserID)
                    .Select(x => x.Friend)
                    .SelectMany(x => x.Posts)
                    .Include(x => x.User).Skip(number).Take(limit).ToList().Select(x => new PostModel
                    {
                        PostId = x.Id,
                        PostText = x.Text,
                        User = new UserModel
                        {
                            Biography = x.User.Biography,
                            Birthdate = x.User.Birthdate,
                            City = x.User.City,
                            First_name = x.User.First_name,
                            Second_name = x.User.Second_name,
                            Id = x.User.Id
                        }
                    }).ToList();
                return posts;
            }
        }

        public void Create(string myUserID, string text)
        {
            var user = applicationContext.Users.FirstOrDefault(x => x.Id == myUserID);
            if (user != null)
            {
                user.Posts.Add(new PostDBModel
                {
                    Text = text,
                });
            }
            applicationContext.SaveChanges();
            updateCache(myUserID);
        }

        public void Update(string myUserID, int postID, string text)
        {
            var post = applicationContext.Posts.FirstOrDefault(x => x.Id == postID && x.userID == myUserID);
            if (post != null)
            {
                post.Text = text;
            }
            applicationContext.SaveChanges();
            updateCache(myUserID);
        }

        public void Delete(string myUserID, int postID)
        {
            var post = applicationContext.Posts.FirstOrDefault(x => x.Id == postID && x.userID == myUserID);
            if (post != null)
            {
                applicationContext.Posts.Remove(post);
            }
            applicationContext.SaveChanges();
            updateCache(myUserID);
        }

        public void SavePostsToCache()
        {
            var users = applicationContext.Users.Include(x => x.Posts).Include(x=>x.Friendships).ToList();
            foreach (var user in users)
            {
                var friends = user.Friendships.Select(x => x.Friend).ToList();

                savePostsToCache(user.Id,friends);
            }

        }

        private void savePostsToCache(string userID, List<UserDBModel> friends) {            
            List<PostModel> retval = new List<PostModel>();

            foreach (var friend in friends)
            {
                var val = friend.Posts.OrderBy(x=>x.Id).Select(x =>
                   new PostModel
                   {
                       PostId = x.Id,
                       PostText = x.Text,
                       User = new UserModel
                       {
                           Biography = friend.Biography,
                           Birthdate = friend.Birthdate,
                           City = friend.City,
                           First_name = friend.First_name,
                           Second_name = friend.Second_name,
                           Id = friend.Id
                       }
                   }).ToList();
                retval.AddRange(val);
            }           
            var key = $"SocialNetwork:User:{userID}";
            var value = JsonConvert.SerializeObject(retval);    
            redisService.SetResource(key, value);
        }

        private void updateCache(string myUserID)
        {
            var me =  applicationContext.Users.Include(x=>x.FriendsOf).FirstOrDefault(x => x.Id == myUserID);
            if (me != null) {
                //те кому я друг
                var userIds = me.FriendsOf.Select(x=>x.userID).ToList();
                var users = applicationContext.Users.Where(x=> userIds.Contains(x.Id)).ToList();
                //идем по каждому и ищем его друзей и обновляем кеш.
                foreach (var user in users)
                {
                    var friends = applicationContext.Friendships.Include(x=>x.Friend).ThenInclude(x=>x.Posts).Where(x => x.userID == user.Id).Select(x => x.Friend).ToList();                   
                    backgroundQueue.Enqueue(async cancellationToken =>
                    {
                        var task = Task.Run(() =>
                        {
                            try
                            {
                                savePostsToCache(user.Id, friends);
                            }
                            catch (Exception ex)
                            {

                            }
                        });
                        await task;
                    });
                }              
            }                                  
        }

    }
}
