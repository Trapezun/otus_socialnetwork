using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Npgsql;
using NpgsqlTypes;
using SocialNetwork.Models;
using System.Buffers;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Configuration;

namespace SocialNetwork.Classes.Services
{
    public class UserService
    {
        private readonly DBService dbService;
        private readonly IConfiguration configuration;

        public UserService(DBService dbService, IConfiguration configuration)
        {
            this.dbService = dbService;
            this.configuration = configuration;
        }


        public UserModel GetByID(string id)
        {
            List<NpgsqlParameter> _params = new List<NpgsqlParameter> {
                new NpgsqlParameter("Id", id),
            };

            var rez = new UserModel();
            dbService.ExecuteSelect("""SELECT "id", "first_name", "second_name", "birthdate", "biography", "city" FROM public.Users WHERE id=(@id)""", async (reader) =>
            {
                while (reader.Read())
                {
                    rez.Id = reader.GetString("id");
                    rez.First_name = reader.GetString("first_name");
                    rez.Second_name = reader.GetString("second_name");
                    rez.Birthdate = reader.GetString("birthdate");
                    rez.Biography = reader.GetString("biography");
                    rez.City = reader.GetString("city");
                }
            }, _params);
            return rez;
        }


        public List<UserModel> UserSearch(string firstName, string lastName)
        {
            if(string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
            {
                return new List<UserModel>();
            }

            List<NpgsqlParameter> _params = new List<NpgsqlParameter> {
                new NpgsqlParameter("first_name", firstName+"%"),
                new NpgsqlParameter("second_name", lastName+"%"),
            };

            var retval = new List<UserModel>();
            var str = """SELECT "id", "first_name", "second_name", "birthdate", "biography", "city" FROM public.Users WHERE second_name ILIKE @second_name AND first_name ILIKE @first_name ORDER BY id""";

            dbService.ExecuteSelect(str, (reader) =>
            {
                while (reader.Read())
                {
                    var rez = new UserModel();
                    rez.Id = reader.GetString("id");
                    rez.First_name = reader.GetString("first_name");
                    rez.Second_name = reader.GetString("second_name");
                    rez.Birthdate = reader.GetString("birthdate");
                    rez.Biography = reader.GetString("biography");
                    rez.City = reader.GetString("city");

                    retval.Add(rez);

                }
            }, _params);


            return retval;
        }


        public async Task<UserModel> Register(UserModelRegisterRequestModel request)
        {
            var passAndSalt = HashPasswordHelper.GetHashPassword(request.Password);

            var id = Guid.NewGuid().ToString();

            List<NpgsqlParameter> _params = new List<NpgsqlParameter> {
                new NpgsqlParameter("Id", id),
                new NpgsqlParameter("First_name", request.First_name),
                new NpgsqlParameter("Second_name", request.Second_name),
                new NpgsqlParameter("Birthdate", request.Birthdate),
                new NpgsqlParameter("Biography", request.Biography),
                new NpgsqlParameter("City", request.City),
                new NpgsqlParameter("PasswordHash", passAndSalt.hash),
                new NpgsqlParameter("PaswordSalt", passAndSalt.salt),
            };

            try
            {
                await dbService.Execute("""INSERT INTO public.users("id", "first_name", "second_name", "birthdate", "biography", "city", "passwordhash", "paswordsalt") VALUES ((@Id), (@First_name), (@Second_name), (@Birthdate), (@Biography), (@City), (@PasswordHash), (@PaswordSalt)) """, _params);
            }
            catch (Exception ex)
            {

            }

            return new UserModel
            {
                Biography = request.Biography,
                Birthdate = request.Birthdate,
                City = request.City,
                First_name = request.First_name,
                Id = id,
                Second_name = request.Second_name
            };
        }





    }
}
