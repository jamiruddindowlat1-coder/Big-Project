using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CourseManagementSystem.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace CourseManagementSystem.Api
{
    public class CheckAccounts
    {
        public static void Run()
        {
             var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));

            using var context = new ApplicationDbContext(optionsBuilder.Options);
            
            Console.WriteLine("--- ACCOUNTS TABLE COUNT ---");
            Console.WriteLine($"Total Transactions: {context.Accounts.Count()}");
            
            var latest = context.Accounts.OrderByDescending(t => t.CreatedAt).Take(5).ToList();
            foreach (var t in latest)
            {
                Console.WriteLine($"ID: {t.Id}, Type: {t.TransactionType}, Category: {t.Category}, Amount: {t.Amount}, Date: {t.Date}, CreatedAt: {t.CreatedAt}");
            }
        }
    }
}
