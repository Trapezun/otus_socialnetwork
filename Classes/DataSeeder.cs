using EFCore.BulkExtensions;
using SocialNetwork.Models;
using System.Globalization;
using System.Reflection;

namespace SocialNetwork.Classes
{
    public class DataSeeder
    {
        public static void Seed(ApplicationContext dbContext) {

            var userCount = dbContext.Users.Count();
            if (userCount > 0) {
                return;
            }

            var fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),"files","people.csv");

            var passAndSalt = HashPasswordHelper.GetHashPassword("Password");

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

                        var id = Guid.NewGuid().ToString();


                        dbUsers.Add(new Models.UserDBModel()
                        {
                            Id = id,
                            Birthdate = dt,
                            City = city,
                            Biography = "Biography",
                            First_name = firslName,
                            Second_name = lastName,
                            PasswordHash = passAndSalt.hash,
                            PaswordSalt = passAndSalt.salt
                        });                                                        
                    }
                }

                dbContext.BulkInsert(dbUsers, new BulkConfig()
                {
                    SetOutputIdentity = true,
                    PreserveInsertOrder = true
                });

            }

            dbContext.SaveChanges();
        }
    }
}
