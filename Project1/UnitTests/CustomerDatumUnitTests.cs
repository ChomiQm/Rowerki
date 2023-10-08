using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project1.Configurations;
using Project1.Controllers;
using Project1.Models;
using Xunit;

namespace Project1.Tests.Controllers
{
    public class CustomerDatumControllerTests
    {
        private readonly DbContextOptions<ModelContext> options;
        private readonly ModelContext _dbContext;

        public CustomerDatumControllerTests()
        {
            options = new DbContextOptionsBuilder<ModelContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new ModelContext(options);
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var password1 = "password1";
            var password2 = "password2";
            var password3 = "password3";

            var customerDatas = new List<CustomerDatum>
            {
                new CustomerDatum
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Name1",
                    CustomerSurrname = "Surrname1",
                    Town = "Town1",
                    Street = "Street1",
                    Estabilishment = "Factorio1",
                    Mail = "mail1@mail1.com",
                    DateOfFirstBuy = DateTime.Now.AddDays(-2),
                    CustomerLogin = "login1",
                    CustomerPassword = BCrypt.Net.BCrypt.HashPassword(password1)
                },

                new CustomerDatum
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Name2",
                    CustomerSurrname = "Surrname2",
                    Town = "Town2",
                    Street = "Street2",
                    Estabilishment = "Factorio2",
                    Mail = "mail2@mail2.com",
                    DateOfFirstBuy = DateTime.Now.AddDays(-7),
                    CustomerLogin = "login2",
                    CustomerPassword = BCrypt.Net.BCrypt.HashPassword(password2)
                },

                new CustomerDatum
                {
                    CustomerId = Guid.NewGuid(),
                    CustomerName = "Name3",
                    CustomerSurrname = "Surrname3",
                    Town = "Town3",
                    Street = "Street3",
                    Estabilishment = "Factorio3",
                    Mail = "mail3@mail3.com",
                    DateOfFirstBuy = DateTime.Now,
                    CustomerLogin = "login3",
                    CustomerPassword = BCrypt.Net.BCrypt.HashPassword(password3)
                }
            };

            _dbContext.CustomerData.AddRange(customerDatas);
            _dbContext.SaveChanges();
        }
    }
}