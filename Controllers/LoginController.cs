using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Classes.Services;
using SocialNetwork.Models;
using System.Net;

namespace SocialNetwork.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> logger;
        private readonly LoginService loginService;

        public LoginController(ILogger<LoginController> logger, LoginService loginService)
        {
            this.logger = logger;
            this.loginService = loginService;
        }

        [HttpPost]
        [Route("/login")]
        [AllowAnonymous]
        public async Task<LoginResponseModel> Login([FromBody] LoginRequestModel model)
        {
            var rez = await loginService.ValidateUser(model);
            if(rez == null)
            {
                return null;
            }
            return rez;            

        }









    }
}


