using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Classes.Services;
using SocialNetwork.Models;
using System.Net;
using System;
using System.Net.WebSockets;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using SocialNetwork.Classes;
using Microsoft.Extensions.DependencyInjection;

namespace SocialNetwork.Controllers
{
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly PostService postService;
        private readonly TokenJWTService tokenJWTService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly NotificationSenderService rabbitMQSenderService;
        private readonly FriendsService friendsService;
        private readonly ILogger<UserController> logger;


        public PostController(PostService postService,
            TokenJWTService tokenJWTService,
            IHttpContextAccessor httpContextAccessor,
            NotificationSenderService rabbitMQSenderService,
            FriendsService friendsService,
            ILogger<UserController> logger)
        {
            this.postService = postService;
            this.tokenJWTService = tokenJWTService;
            this.httpContextAccessor = httpContextAccessor;
            this.rabbitMQSenderService = rabbitMQSenderService;
            this.friendsService = friendsService;
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
            postService.Create(myID, text);
            sendNewMessageSignal(myID);
        }


        [HttpPut]
        [Route("/post/update")]
        public void Update(int postID, string text)
        {
            var myID = tokenJWTService.GetID(httpContextAccessor);
            postService.Update(myID, postID, text);
            sendNewMessageSignal(myID);

        }


        [HttpDelete]
        [Route("/post/delete")]
        public void Delete(int postID)
        {
            var myID = tokenJWTService.GetID(httpContextAccessor);
            postService.Delete(myID, postID);
            sendNewMessageSignal(myID);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("post/feed/posted")]
        public async Task FeedPosted([FromServices] WebSocketConnectionManager connectionManager            )
        {

            var context = this.httpContextAccessor.HttpContext;                                           
            if (context.WebSockets.IsWebSocketRequest)
            {
                var myID = tokenJWTService.GetID(httpContextAccessor);

                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();

                if (webSocket.State == WebSocketState.Open)
                {
                    connectionManager.AddSocket(webSocket, myID);
                    await connectionManager.ListenSocketAsync(webSocket, myID);
                }
                else {
                    connectionManager.RemoveSocket(myID);
                }                                
            }
            else
            {                
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }


        private void sendNewMessageSignal(string userID)
        {
            var friendsOfMe = friendsService.FriendsOf(userID);
            foreach (var item in friendsOfMe)
            {
                rabbitMQSenderService.SendMessage(new NotificationSenderService.SendMessageModel {
                    ToUserID = item 
                });
            }


           
        }


    }
}



