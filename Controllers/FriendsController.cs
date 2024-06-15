using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Classes.Services;
using SocialNetwork.Models;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class FriendsController : ControllerBase
    {
        private readonly FriendsService friendsService;
        private readonly TokenJWTService tokenJWTService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<UserController> logger;
        

        public FriendsController(FriendsService friendsService, 
            TokenJWTService tokenJWTService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserController> logger)
        {
            this.friendsService = friendsService;
            this.tokenJWTService = tokenJWTService;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;            
        }

        [HttpGet]       
        [Route("/friends")]
        public List<UserModel> List()
        {
            var myID = tokenJWTService.GetID(httpContextAccessor);
             var list  = friendsService.List(myID);
            return list;
        }

        [HttpPut]        
        [Route("/friend/set/{user_id}")]        
        public void Set(string user_id)
        {
            var myID = tokenJWTService.GetID(httpContextAccessor);

            friendsService.Set(myID, user_id);                       
        }

        [HttpDelete]        
        [Route("/friend/delete/{user_id}")]
        public void Delete(string user_id)
        {
            var myID = tokenJWTService.GetID(httpContextAccessor);

            friendsService.Delete(myID, user_id);
        }

    }
}



