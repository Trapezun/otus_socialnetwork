using DalSoft.Hosting.BackgroundQueue.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SocialNetwork.Classes;
using SocialNetwork.Classes.Redis;
using SocialNetwork.Classes.Services;
using SocialNetwork.Extensions;
using StackExchange.Redis;
using System.Net.WebSockets;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "Authorization",
        Description = "Copy 'Bearer ' + valid JWT token into field",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
});


builder.Services.AddDbContext<ApplicationContext>();


// Check if the "migrate" argument is passed
if (args.Length > 0 && args[0].ToLower() == "migrate")
{
    Console.WriteLine("Starting migration...");

    var appMigrate = builder.Build();

    using (var scope = appMigrate.Services.CreateScope())
    {
        using (var dbContext = scope.ServiceProvider.GetService<ApplicationContext>())
        {
            //dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

          

            DataSeeder.SeedUsers(dbContext);
            DataSeeder.SeedFriends(dbContext);
            DataSeeder.SeedPosts(dbContext);


            Console.WriteLine("Migration completed.");
            return; // Exit after migration          
        }
    }        
}

builder.Services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

builder.Services.AddScoped<DBService>();

builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<FriendsService>();

builder.Services.AddScoped<NotificationSenderService>();



builder.Services.AddHostedService<RabbitMQHostedService>();



builder.Services.AddSingleton<WebSocketConnectionManager>();


builder.Services.AddSingleton<CacheService>();




builder.Services.AddSingleton(x =>
{
    var configuration = x.GetRequiredService<IConfiguration>();

    var redisLocal = configuration.GetConnectionString("redisconnection");

    string redisTlsCert = configuration.GetValue<string>("redis-tls-cert");
    string redisTlsKey = configuration.GetValue<string>("redis-tls-key");

    var connectionMultiplexer = new ConnectionMultiplexerCreator(redisLocal, redisTlsCert, redisTlsKey);
    var retval = new RedisWrapper(connectionMultiplexer);
    return retval;
});

builder.Services.AddSingleton(x =>
{
    var wrapper = x.GetRequiredService<RedisWrapper>();
    return new RedisService(wrapper);    
});

var proxySection = builder.Configuration.GetSection("JWTAuthorization");
var issuer = proxySection.GetValue<string>("issuer") ?? "issuer";
var audience = proxySection.GetValue<string>("audience") ?? throw new Exception("no audience");
var encryptingKey = proxySection.GetValue<string>("encryptingKey") ?? throw new Exception("no encryptingKey");
var signingKey = proxySection.GetValue<string>("signingKey") ?? throw new Exception("no signingKey");

builder.Services.AddJWTAuthorization(builder.Environment, issuer, audience, encryptingKey, signingKey);

builder.Services.AddScoped<TokenJWTService>(x =>
{
    return new TokenJWTService(issuer, audience, encryptingKey, signingKey);
});


builder.Services.AddBackgroundQueue(onException: exception => { });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    using (var dbContext = scope.ServiceProvider.GetService<ApplicationContext>())
    {
        //dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        DataSeeder.SeedUsers(dbContext);
        DataSeeder.SeedFriends(dbContext);
        DataSeeder.SeedPosts(dbContext);
    }
}

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    //app.UseSwagger();
    //app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseWebSockets();



//app.UseEndpoints(endpoints =>
//{
//    endpoints.Map("/ws", async context =>
//    {
//        if (context.WebSockets.IsWebSocketRequest)
//        {
//            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
//            //await HandleWebSocketAsync(webSocket);
//        }
//        else
//        {
//            context.Response.StatusCode = 400;
//        }
//    });
//});

app.Run();

