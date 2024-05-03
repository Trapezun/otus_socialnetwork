using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Npgsql;
using NpgsqlTypes;
using SocialNetwork.Models;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;

namespace SocialNetwork.Classes.Services
{
    public class UserService
    {
        private readonly DBService dbService;
        private readonly TokenJWTService tokenJWTService;
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserService(DBService dbService, TokenJWTService tokenJWTService, IHttpContextAccessor httpContextAccessor)
        {
            this.dbService = dbService;
            this.tokenJWTService = tokenJWTService;
            this.httpContextAccessor = httpContextAccessor;
        }

   
        public async Task<UserModel> GetByID(string id)
        {            
            List<NpgsqlParameter> _params = new List<NpgsqlParameter> {
                new NpgsqlParameter("Id", id),                
            };

            var rez = new UserModel();
            await dbService.ExecuteSelect("""SELECT "id", "first_name", "second_name", "birthdate", "biography", "city" FROM public.Users WHERE id=(@id)""", async (reader) =>
            {
                while (await reader.ReadAsync())
                {
                    rez.Id = reader.GetString("id");
                    rez.First_name = reader.GetString("first_name");
                    rez.Second_name = reader.GetString("second_name");
                    rez.Birthdate = reader.GetString("birthdate");
                    rez.Biography = reader.GetString("biography");
                    rez.City = reader.GetString("city");                   
                }
            },  _params);
            return rez;            
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
                new NpgsqlParameter("PasswordHash", passAndSalt.hashed),
                new NpgsqlParameter("PaswordSalt", passAndSalt.salt),
            };

            try
            {
                await dbService.Execute("""INSERT INTO public.users("id", "first_name", "second_name", "birthdate", "biography", "city", "passwordhash", "paswordsalt") VALUES ((@Id), (@First_name), (@Second_name), (@Birthdate), (@Biography), (@City), (@PasswordHash), (@PaswordSalt)) """, _params);
            }catch (Exception ex)
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
