using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Classes.Services;
using SocialNetwork.Models;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly PostService postService;
        private readonly TokenJWTService tokenJWTService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<UserController> logger;
        

        public PostController(PostService postService,
            TokenJWTService tokenJWTService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserController> logger)
        {
            this.postService = postService;
            this.tokenJWTService = tokenJWTService;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;            
        }

        [HttpGet]
        [Route("/post/feed")]        
        public List<PostModel> Feed(int number, int limit)
        {
            var myID = tokenJWTService.GetID(httpContextAccessor);
            var retval = postService.Feed(myID, number, limit);
            return retval;
        }


        [HttpPost]
        [Route("/post/create")]
        public void Create(string text)
        {
            var myID = tokenJWTService.GetID(httpContextAccessor);
            postService.Create(myID,text);

        }


        [HttpPut]
        [Route("/post/update")]
        public void Delete(int postID, string text)
        {
            var myID = tokenJWTService.GetID(httpContextAccessor);
            postService.Update(myID, postID, text);

        }


        [HttpDelete]
        [Route("/post/delete")]        
        public void Delete(int postID)
        {
            var myID = tokenJWTService.GetID(httpContextAccessor);
            postService.Delete(myID, postID);
            
        }

    }
}



