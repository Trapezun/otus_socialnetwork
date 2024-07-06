using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SocialNetwork.Classes.Services
{
    public class RabbitMQHostedService : BackgroundService
    {
        private readonly IConfiguration configuration;
        private readonly IServiceScopeFactory serviceScopeFactory;

        public RabbitMQHostedService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory)
        {
            this.configuration = configuration;
            this.serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            var factory = new ConnectionFactory
            {
                HostName = configuration[Constants.RabbitMqConfigHostName],
                UserName = configuration[Constants.RabbitMqConfigUserName],
                Password = configuration[Constants.RabbitMqConfigPassword]
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: Constants.RabbitMqQueuePostNew, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var userID = Encoding.UTF8.GetString(body);

                using (var scope = serviceScopeFactory.CreateScope())
                {
                    // Обработка полученного сообщения
                    var postService = scope.ServiceProvider.GetService<PostService>();
                    postService.SavePostsToCache(userID);

                    var webSocketConnectionManager = scope.ServiceProvider.GetService<WebSocketConnectionManager>();
                    await webSocketConnectionManager.SendMessageToAllAsync(userID, Constants.WebSocketNewPost);
                }
            };

            channel.BasicConsume(queue: Constants.RabbitMqQueuePostNew, autoAck: true, consumer: consumer);

            // Keep the task running
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
