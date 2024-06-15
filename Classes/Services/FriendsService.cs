using Microsoft.EntityFrameworkCore;
using SocialNetwork.Models;

namespace SocialNetwork.Classes.Services
{
    public class FriendsService
    {
        private readonly ApplicationContext applicationContext;

        public FriendsService(ApplicationContext applicationContext)
        {
            this.applicationContext = applicationContext;
        }

        public List<UserModel> List(string myUserID)
        {         
            var friends = applicationContext.Friendships
                .Where(f => f.userID == myUserID)
                .Select(f => f.Friend)
                .ToList();


            return friends.Select(x => new UserModel
            {
                Biography = x.Biography,
                Birthdate = x.Birthdate,
                City = x.City,
                First_name=x.First_name,
                Id = x.Id,
                Second_name = x.Second_name,
            }).ToList();              
        }

        public void Set(string toWhomUserID, string friendID) {

            var friendship = new FriendshipDBModel
            {
                userID = toWhomUserID,
                friendID = friendID
            };

            applicationContext.Friendships.Add(friendship);
            applicationContext.SaveChanges();          
        }

        public void Delete(string toWhomUserID, string friendID)
        {
            var friendship = applicationContext.Friendships.FirstOrDefault(f => f.userID == toWhomUserID && f.friendID == friendID);

            if (friendship != null)
            {
                applicationContext.Friendships.Remove(friendship);
                applicationContext.SaveChanges();
            }
        }
    }
}
