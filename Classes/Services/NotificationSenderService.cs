using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace SocialNetwork.Classes.Services
{
    public class NotificationSenderService
    {
        private readonly IConfiguration configuration;

        private readonly string _hostname;
        private readonly string _username;
        private readonly string _password;

        public NotificationSenderService(IConfiguration configuration)
        {
            this.configuration = configuration;

            _hostname = configuration[Constants.RabbitMqConfigHostName];
            _username = configuration[Constants.RabbitMqConfigUserName];
            _password = configuration[Constants.RabbitMqConfigPassword];
        }

        public void SendMessage(SendMessageModel message)
        {
            var queue = $"{Constants.RabbitMqQueuePostNew}";
            
            var factory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: queue, durable: false, exclusive: false, autoDelete: false, arguments: null);                                                          
                var body = Encoding.UTF8.GetBytes(message.ToUserID.ToString());

                channel.BasicPublish(exchange: "",
                               routingKey: queue,
                               basicProperties: null,
                               body: body);
            }
        }

        public class SendMessageModel
        {
            public string ToUserID { get; set; }
        }



    }











}

