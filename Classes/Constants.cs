namespace SocialNetwork.Classes
{
    public static class Constants
    {
        public static readonly TimeSpan UserCacheExpiry = TimeSpan.FromMinutes(5);

        public const string RabbitMqExchangePostNew = "ExchangePostNew";
        public const string RabbitMqQueuePostNew = "QueuePostNew";

        public const string RabbitMqConfigHostName = "RabbitMQ:HostName";
        public const string RabbitMqConfigUserName = "RabbitMQ:UserName";
        public const string RabbitMqConfigPassword = "RabbitMQ:Password";


        public const string WebSocketNewPost = "NewPost";

    }
}
