using EFCore.BulkExtensions;
using SocialNetwork.Models;
using System.Globalization;
using System.Reflection;

namespace SocialNetwork.Classes
{
    public class DataSeeder
    {
        public static void SeedUsers(ApplicationContext dbContext)
        {

            var userCount = dbContext.Users.Count();
            if (userCount > 0)
            {
                return;
            }

            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "files", "people.csv");

            var passAndSalt = HashPasswordHelper.GetHashPassword("1");

            using (StreamReader sr = File.OpenText(fileName))
            {
                List<UserDBModel> dbUsers = new List<UserDBModel>();

                string s = String.Empty;
                while ((s = sr.ReadLine()) != null)
                {
                    var mas = s.Trim().Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (mas.Length >= 2)
                    {
                        var lastName = mas[0].Split(' ')[0];
                        var firslName = mas[0].Split(' ')[1];

                        var dateOfBirthStr = mas[1];
                        var dateOfBirth = DateTime.ParseExact(dateOfBirthStr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                        var dt = dateOfBirth.ToString("dd-MM-yyyy");

                        var city = mas[2];

                        var userID = Guid.NewGuid().ToString();

                        var user = new Models.UserDBModel()
                        {
                            Id = userID,
                            Birthdate = dt,
                            City = city,
                            Biography = "Biography",
                            First_name = firslName,
                            Second_name = lastName,
                            PasswordHash = passAndSalt.hash,
                            PaswordSalt = passAndSalt.salt,
                        };

                        dbUsers.Add(user);
                    }
                }



                dbContext.BulkInsert(dbUsers, new BulkConfig()
                {
                    BulkCopyTimeout = 0,
                    //SetOutputIdentity = true,
                    //PreserveInsertOrder = true
                });



            }


        }


        public static void SeedFriends(ApplicationContext dbContext)
        {
            if (dbContext.Friendships.Count() > 0) {
                return;
            }

            var allUsers = dbContext.Users.ToList();
            int numUsers = allUsers.Count;
            int minFriends = 1;
            int maxFriends = 3;
            Random random = new Random();

            List<FriendshipDBModel> dbFriendship = new List<FriendshipDBModel>();

            foreach (var user in allUsers)
            {
                int numFriends = random.Next(minFriends, maxFriends + 1);

                for (int i = 0; i < numFriends; i++)
                {
                    int friendId = random.Next(1, numUsers + 1);

                    var friend = allUsers.Skip(friendId - 1).Take(1).First();
                    if (friend.Id != user.Id)
                    {
                        if (dbFriendship.Any(x => x.userID == user.Id && x.friendID == friend.Id))
                        {
                            continue;
                        }
                        else
                        {
                            var friendship = new FriendshipDBModel
                            {
                                userID = user.Id,
                                friendID = friend.Id
                            };

                            dbFriendship.Add(friendship);                           
                        }
                    }
                }
            }

            dbContext.BulkInsert(dbFriendship, new BulkConfig()
            {
                BulkCopyTimeout = 0,                
            });                       
        }



        public static void SeedPosts(ApplicationContext dbContext)
        {
            if (dbContext.Posts.Count() > 0)
            {
                return;
            }

            List<PostDBModel> dbPosts = new List<PostDBModel>();

            var allUsers = dbContext.Users.ToList();

            foreach (var user in allUsers)
            {
                for (int i = 0; i < 3; i++)
                {
                    dbPosts.Add(new PostDBModel
                    {
                        userID = user.Id,
                        Text = $"Text {i} from {user.First_name} {user.Second_name}."
                    });
                }
            }


            dbContext.BulkInsert(dbPosts, new BulkConfig()
            {
                BulkCopyTimeout = 0,
            });

        }

    }
}
