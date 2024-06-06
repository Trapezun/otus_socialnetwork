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
        [AllowAnonymous]
        public UserModel UserGet(string id)
        {
            var retval = userService.GetByID(id);
            return retval;
        }


        [HttpGet]
        [Route("/user/search/")]
        [AllowAnonymous]
        public List<UserModel> UserSearch([FromQuery]string firstName, [FromQuery] string lastName)
        {
            var retval = userService.UserSearch(firstName, lastName);
            return retval;
        }



        [HttpPost]
        [Route("/user/register")]
        [AllowAnonymous]
        public async Task<UserModel> Register([FromBody] UserModelRegisterRequestModel model)
        {
            var retval = await userService.Register(model);
            return retval;        
        }
    }
}



