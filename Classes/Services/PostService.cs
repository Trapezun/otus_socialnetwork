using DalSoft.Hosting.BackgroundQueue;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
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
        
        private readonly CacheService cacheService;

        public PostService(ApplicationContext applicationContext,        
            CacheService cacheService
            
            )
        {
            this.applicationContext = applicationContext;            
            this.cacheService = cacheService;
        }



        public List<PostModel> Feed(string myUserID, int number, int limit)
        {
            var value = cacheService.GetPosts(myUserID);
            if (value != null)
            {
                var retval = JsonConvert.DeserializeObject<List<PostModel>>(value);
                if (retval.Count != 0)
                {
                    cacheService.UpdatePostsTime(myUserID);

                    retval = retval.Skip(number).Take(limit).ToList();
                    return retval;
                }
            }

            var user = applicationContext.Users.Include(x => x.Friendships).ThenInclude(x => x.Friend).ThenInclude(x => x.Posts).FirstOrDefault(x => x.Id == myUserID);
            if (user != null)
            {
                var friends = user.Friendships.Select(x => x.Friend).ToList();
                var allPosts = getPostsByFriends(friends);

                cacheService.SavePostsToCache(myUserID, allPosts);

                var posts = allPosts.Skip(number).Take(limit).ToList().Select(x => new PostModel
                {
                    PostId = x.PostId,
                    PostText = x.PostText,
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
            return new List<PostModel> { };

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
            
        }

        public void Update(string myUserID, int postID, string text)
        {
            var post = applicationContext.Posts.FirstOrDefault(x => x.Id == postID && x.userID == myUserID);
            if (post != null)
            {
                post.Text = text;
            }
            applicationContext.SaveChanges();
            
        }

        public void Delete(string myUserID, int postID)
        {
            var post = applicationContext.Posts.FirstOrDefault(x => x.Id == postID && x.userID == myUserID);
            if (post != null)
            {
                applicationContext.Posts.Remove(post);
            }
            applicationContext.SaveChanges();            
        }

        public void SaveAllPostsToCache()
        {
            var users = applicationContext.Users.Include(x => x.Posts).Include(x => x.Friendships).ToList();
            foreach (var user in users)
            {
                var friends = user.Friendships.Select(x => x.Friend).ToList();

                var posts = getPostsByFriends(friends);
                cacheService.SavePostsToCache(user.Id, posts);
            }
        }


        public void SavePostsToCache(string userID)
        {
            var friends = applicationContext.Friendships.Include(x => x.Friend).ThenInclude(x => x.Posts).Where(x => x.userID == userID).Select(x => x.Friend).ToList();
            var posts = getPostsByFriends(friends);
            cacheService.SavePostsToCache(userID, posts);
        }



        private List<PostModel> getPostsByFriends(List<UserDBModel> friends)
        {

            var retval = new List<PostModel>();
            foreach (var friend in friends)
            {
                var val = friend.Posts.OrderBy(x => x.Id).Select(x =>
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
            return retval;
        }


     





    }
}
