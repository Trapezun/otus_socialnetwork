using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Classes.Services;
using SocialNetwork.Models;
using System.Net;

namespace SocialNetwork.Controllers
{
    //public class LoginController : ControllerBase
    //{
    //    private readonly ILogger<LoginController> logger;
    //    private readonly LoginService loginService;

    //    public LoginController(ILogger<LoginController> logger, LoginService loginService)
    //    {
    //        this.logger = logger;
    //        this.loginService = loginService;
    //    }

    //    [HttpPost]
    //    [Route("/login")]
    //    [AllowAnonymous]
    //    public LoginResponseModel Login([FromBody] LoginRequestModel model)
    //    {
    //        var rez = loginService.ValidateUser(model);
    //        if (rez == null)
    //        {
    //            return null;
    //        }
    //        return rez;

    //    }

    //}

    public class TestController : ControllerBase
    {

        //[HttpGet]
        //[Route("/test")]
        //[AllowAnonymous]
        //public async Task<string> Test()
        //{
        //    return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList[0].ToString();
        //}

        [HttpGet]
        [Route("/health")]
        [AllowAnonymous]
        public async Task<string> Health()
        {
            return "\"status\": \"OK\"";

        }

    }
}


