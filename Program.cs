using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SocialNetwork.Classes;
using SocialNetwork.Classes.Services;
using SocialNetwork.Extensions;
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


//var proxySection = builder.Configuration.GetSection("JWTAuthorization");
//var issuer = proxySection.GetValue<string>("issuer") ?? "issuer";
//var audience = proxySection.GetValue<string>("audience") ?? throw new Exception("no audience");
//var encryptingKey = proxySection.GetValue<string>("encryptingKey") ?? throw new Exception("no encryptingKey");
//var signingKey = proxySection.GetValue<string>("signingKey") ?? throw new Exception("no signingKey");

//builder.Services.AddJWTAuthorization(builder.Environment, issuer, audience, encryptingKey, signingKey);

//builder.Services.AddSingleton<Microsoft.AspNetCore.Http.IHttpContextAccessor, Microsoft.AspNetCore.Http.HttpContextAccessor>();

//builder.Services.AddDbContext<ApplicationContext>();
//builder.Services.AddScoped<DBService>();

//builder.Services.AddScoped<LoginService>();
//builder.Services.AddScoped<UserService>();

//builder.Services.AddScoped<TokenJWTService>(x =>
//{
//    return new TokenJWTService(issuer, audience, encryptingKey, signingKey);
//});



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    using (var dbContext = scope.ServiceProvider.GetService<ApplicationContext>())
    {
        //dbContext.Database.EnsureDeleted();
        //dbContext.Database.EnsureCreated();

        //DataSeeder.Seed(dbContext);
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

app.Run();
