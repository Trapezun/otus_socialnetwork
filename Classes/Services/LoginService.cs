using Npgsql;
using SocialNetwork.Models;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace SocialNetwork.Classes.Services
{
    public class LoginService
    {
        private readonly DBService dbService;
        private readonly TokenJWTService tokenJWTService;

        public LoginService(DBService dbService, TokenJWTService tokenJWTService)
        {
            this.dbService = dbService;
            this.tokenJWTService = tokenJWTService;
        }

        public async Task<LoginResponseModel?> ValidateUser(LoginRequestModel request)
        {            
            List<NpgsqlParameter> _params = new List<NpgsqlParameter> {
                new NpgsqlParameter("Id", request.Id),                
            };

            string? token = null;

            await this.dbService.ExecuteSelect("""SELECT "id", "passwordhash", "paswordsalt" FROM public.Users WHERE id=(@id)""",
                async (reader) =>
                {
                    while (await reader.ReadAsync() && token==null)
                    {
                        var id = reader.GetString("id");                        
                        var passwordhash = (byte[])reader["passwordhash"];                         
                        var paswordsalt = (byte[])reader["paswordsalt"]; 
                        var isValid  =  HashPasswordHelper.ValidatePassword(request.Password, paswordsalt,passwordhash);
                        if (isValid)
                        {
                            token = tokenJWTService.CreateToken(id, TimeSpan.FromMinutes(20));
                        }
                    }
                }, _params);

            if (!string.IsNullOrEmpty(token)) {
                return new LoginResponseModel { Token = token };
            }
            return null;            
        }

    }
}
