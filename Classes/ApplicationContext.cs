using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SocialNetwork.Models;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace SocialNetwork.Classes
{
    public class ApplicationContext : DbContext
    {
        private readonly IConfiguration configuration;

        public DbSet<UserDBModel> Users { get; set; } = null!;
        public DbSet<PostDBModel> Posts { get; set; } = null!;

        public DbSet<FriendshipDBModel> Friendships { get; set; } = null!;


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

        {// Настройка первичных ключей для модели Friendship
            modelBuilder.Entity<FriendshipDBModel>()
                .HasKey(f => new { f.userID, f.friendID });

            // Настройка отношений для модели Friendship
            modelBuilder.Entity<FriendshipDBModel>()
                .HasOne(f => f.User)
                .WithMany(u => u.Friendships)
                .HasForeignKey(f => f.userID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendshipDBModel>()
                .HasOne(f => f.Friend)
                .WithMany(u => u.FriendsOf)
                .HasForeignKey(f => f.friendID)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
