
using SocialNetwork.Classes.Services;

namespace SocialNetwork.Classes
{
    public class CacheWorker : BackgroundService
    {
        private readonly CacheService cacheService;
        private readonly IServiceScopeFactory scopeFactory;

        public CacheWorker(CacheService cacheService, IServiceScopeFactory scopeFactory)
        {
            this.cacheService = cacheService;
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Выполняем задачу пока не будет запрошена остановка приложения
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {                    
                    await updateCache();
                }
                catch (Exception ex)
                {
                    // обработка ошибки однократного неуспешного выполнения фоновой задачи
                }

                await Task.Delay(1000);
            }          
        }


        private async Task updateCache() {           
            var userID = cacheService.GetUserForHandle();
            if (!string.IsNullOrEmpty(userID))                       
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    using (var dbContext = scope.ServiceProvider.GetService<ApplicationContext>())
                    {
                        var postService = new PostService(dbContext, cacheService);
                        postService.SavePostsToCache(userID);                        
                    }
                }
            }
        }

    }
}
