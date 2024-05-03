using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialNetwork.Models;

namespace SocialNetwork.Classes
{   
    public class ApplicationContext : DbContext
    {
        private readonly IConfiguration configuration;

        public DbSet<UserDBModel> Users { get; set; } = null!;


        public ApplicationContext(IConfiguration configuration)
        {
            this.configuration = configuration;            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            string connection = configuration.GetConnectionString("dbconnection");            
            optionsBuilder.UseNpgsql(connection);
            
            //optionsBuilder.LogTo(Console.WriteLine);           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }

    }
}
