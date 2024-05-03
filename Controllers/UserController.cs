using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Classes.Services;
using SocialNetwork.Models;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> logger;
        private readonly UserService userService;

        public UserController(ILogger<UserController> logger, UserService userService)
        {
            this.logger = logger;
            this.userService = userService;
        }

        [HttpGet]
        [Route("/user/get/{id}")]
        public async Task<UserModel> UserGet(string id)
        {
            var retval = await userService.GetByID(id);
            return retval;
        }


        [HttpPost]
        [Route("/user/register")]
        [AllowAnonymous]
        public async Task<UserModel> Register([FromBody] UserModelRegisterRequestModel model)
        {
            var retval = await userService.Register(model);
            return retval;

            //try
            //{

            //    var retval = await userService.Register(model);
            //    return retval;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    //logger.LogError(ex.Message);
            //    return null;
            //}
        }
    }
}



