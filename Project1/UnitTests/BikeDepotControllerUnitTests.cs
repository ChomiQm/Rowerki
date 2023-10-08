using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Project1.Controllers;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Moq.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Linq;
using Xunit.Sdk;
using Project1.Configurations;

namespace Project1.UnitTests
{


    public class BikeDepotControllerTests
    {
        private readonly Mock<ModelContext> _contextMock;
        private readonly BikeDepotController _controller;

        public BikeDepotControllerTests()
        {
            // Mock of DbContext creator
            _contextMock = new Mock<ModelContext>(new DbContextOptions<ModelContext>());
            _controller = new BikeDepotController(_contextMock.Object);
        }

        [Fact]
        public async Task AddBikeDepot_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var bikeDepot = new BikeDepot
            {
                BikeName = "Bike 1",
                BikeDescription = "Description 1",
                Quantity = 10,
                DateOfStore = DateTime.Now,
                ManuDepSeries = null
            };
            _contextMock.Setup(c => c.BikeDepots.AddAsync(It.IsAny<BikeDepot>(), default))
                        .Callback<BikeDepot, CancellationToken>((b, _) => bikeDepot = b);
            _controller.ModelState.AddModelError("Price", "Price must be greater than zero.");
            _controller.ModelState.AddModelError("ManuDepSeriesId", "Invalid Manu Department Series ID");
            // Act
            var result = await _controller.AddBikeDepot(bikeDepot);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }
        [Fact]
        public async Task AddBikeDepot_ShouldReturnBadRequest_WhenBikeDepotIsNull()
        {
            // Arrange 
            BikeDepot? bikeDepot = null;


            _contextMock.Setup(c => c.BikeDepots.AddAsync(It.IsAny<BikeDepot>(), default))
                .Callback<BikeDepot, CancellationToken>((b, _) => bikeDepot = b);

            #pragma warning disable CS8604
            // Act
            var result = await _controller.AddBikeDepot(bikeDepot);
            #pragma warning restore CS8604
            // Assert 
            Assert.IsType<BadRequestResult>(result);
        }
         

        [Fact]
        public async Task AddBikeDepot_ShouldAddBikeDepotToDatabase_WhenModelStateIsValid()
        {
            // Arrange
            var bikeDepot = new BikeDepot
            {
                BikeName = "Mountain Bike",
                BikeDescription = "A bike for rough terrain",
                Quantity = 5,
                DateOfStore = DateTime.Now,
                ManuDepSeriesId = 3,
                Price = 1000,
                ManuDepSeries = null
            };

            var bikeDepots = new List<BikeDepot>();
            _contextMock.Setup(c => c.BikeDepots).ReturnsDbSet(bikeDepots);

            // Assert before
            Assert.Empty(bikeDepots);

            // Act
            var result = await _controller.AddBikeDepot(bikeDepot);

            // Assert after
            bikeDepots.Add(bikeDepot); // add to list
            Assert.Contains(bikeDepot, bikeDepots);

        }

        [Fact]
        public async Task AddBikeDepot_ShouldReturnCorrectActionResult_WhenModelStateIsValid()
        {
            // Arrange
            var bikeDepot = new BikeDepot
            {
                BikeName = "Mountain Bike",
                BikeDescription = "A bike for rough terrain",
                Quantity = 5,
                DateOfStore = DateTime.Now,
                ManuDepSeriesId = 3,
                Price = 1000,
                ManuDepSeries = null
            };

            var bikeDepots = new List<BikeDepot>();
            _contextMock.Setup(c => c.BikeDepots).ReturnsDbSet(bikeDepots);

            var controller = new BikeDepotController(_contextMock.Object);

            // Act
            var result = await _controller.AddBikeDepot(bikeDepot);

            // Assert
            Assert.IsType<CreatedAtActionResult>(result);

            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.NotNull(createdAtActionResult);
            Assert.Equal("GetBikeDepot", createdAtActionResult.ActionName);

        }

        [Fact]
        public async Task DeleteBikeDepot_ReturnsNotFound_WhenBikeDepotDoesNotExist()
        {
            #pragma warning disable CS8600
            // Arrange
            _contextMock.Setup(c => c.BikeDepots.FindAsync(4)).ReturnsAsync((BikeDepot)null);
            #pragma warning restore CS8600

            // Act
            var result = await _controller.DeleteBikeDepot(4);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task DeleteBikeDepot_RemovesBikeDepotFromDatabase_WhenBikeDepotExists()
        {
            // Arrange
            var bikeDepot = new BikeDepot
            {
                BikeId = 4,
                BikeName = "Bike 1",
                BikeDescription = "Description 1",
                Quantity = 150,
                DateOfStore = DateTime.Now.AddDays(-3),
                ManuDepSeriesId = 1,
                Price = 1200,
                ManuDepSeries = null
            };
            _contextMock.Setup(c => c.BikeDepots.FindAsync(bikeDepot.BikeId)).ReturnsAsync(bikeDepot);
            _contextMock.Setup(c => c.BikeDepots.Remove(bikeDepot)).Verifiable();
            _contextMock.Setup(c => c.SaveChangesAsync(default)).Returns(Task.FromResult(1));

            // Act
            var result = await _controller.DeleteBikeDepot(bikeDepot.BikeId);

            // Assert
            _contextMock.Verify(c => c.BikeDepots.FindAsync(bikeDepot.BikeId), Times.Once());
            _contextMock.Verify(c => c.BikeDepots.Remove(bikeDepot), Times.Once());
            _contextMock.Verify(c => c.SaveChangesAsync(default), Times.Once());
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(204, noContentResult.StatusCode);
        }
        [Fact]
        public async Task GetAllBikeDepots_ReturnsOkResult_WithListOfBikeDepots()
        {
            // Arrange
            var bikeDepots = new List<BikeDepot>
        {
            new BikeDepot
            {
                BikeId = 1,
                BikeName = "Bike 1",
                BikeDescription = "Description 1",
                Quantity = 150,
                DateOfStore = DateTime.Now.AddDays(-3),
                ManuDepSeriesId = 1,
                Price = 1200
            },
            new BikeDepot
            {
                BikeId = 2,
                BikeName = "Bike 2",
                BikeDescription = "Description 2",
                Quantity = 1500,
                DateOfStore = DateTime.Now.AddDays(-7),
                ManuDepSeriesId = 2,
                Price = 3200
            },
            new BikeDepot
            {
                BikeId = 3,
                BikeName = "Bike 3",
                BikeDescription = "Description 3",
                Quantity = 12500,
                DateOfStore = DateTime.Now.AddDays(-15),
                ManuDepSeriesId = 3,
                Price = 3100
            }
        };
            _contextMock.Setup(c => c.BikeDepots).ReturnsDbSet(bikeDepots);

            // Act
            var result = await _controller.GetAllBikeDepots(); //get all records

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultBikeDepots = Assert.IsAssignableFrom<List<BikeDepot>>(okObjectResult.Value); //assert as get all records
            Assert.Equal(bikeDepots, resultBikeDepots);
        }

        [Fact]
        public async Task GetBikeDepot_ReturnsOkResult_WithBikeDepot()
        {
            // Arrange
            var bikeDepot = new BikeDepot
            {
                BikeId = 1,
                BikeName = "Bike 1",
                BikeDescription = "Description 1",
                Quantity = 150,
                DateOfStore = DateTime.Now.AddDays(-3),
                ManuDepSeriesId = 1,
                Price = 1200
            };
            _contextMock.Setup(c => c.BikeDepots.FindAsync(1)).ReturnsAsync(bikeDepot);

            // Act
            var actionResult = await _controller.GetBikeDepot(1); //getting single record

            // Assert
            var okObjectResult = actionResult.Result as OkObjectResult;
            Assert.IsType<OkObjectResult>(okObjectResult); //assert as get single record
        }

        [Fact]
        public async Task GetBikeDepot_ReturnsNotFound_WhenBikeDepotDoesNotExist()
        {
            // Arrange
            _contextMock.Setup(c => c.BikeDepots.FindAsync(4)).ReturnsAsync((BikeDepot)null);

            // Act
            var result = await _controller.GetBikeDepot(4);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetAllBikeDepots_ReturnsEmptyList_WhenNoBikeDepotsExist()
        {
            // Arrange
            _contextMock.Setup(c => c.BikeDepots).ReturnsDbSet(Array.Empty<BikeDepot>());

            // Act
            var result = await _controller.GetAllBikeDepots();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result.Result);
            var resultBikeDepots = Assert.IsAssignableFrom<List<BikeDepot>>(okObjectResult.Value);
            Assert.Empty(resultBikeDepots);
        }
        [Fact]
        public async Task GetBikeDepot_ReturnsNotFound_ForNonExistingRecord()
        {
            // Arrange
            int nonExistingId = 999;
            _contextMock.Setup(x => x.BikeDepots.FindAsync(nonExistingId)).ReturnsAsync((BikeDepot)null);

            // Act
            var result = await _controller.GetBikeDepot(nonExistingId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateBikeDepot_ReturnsNotFound_ForNonExistingRecord()
        {
            // Arrange
            int nonExistingId = 999;
            _contextMock.Setup(x => x.BikeDepots.FindAsync(nonExistingId)).ReturnsAsync((BikeDepot)null);
            var jsonString = @"{
                ""BikeName"": ""Bike 1 updated"",
                ""Quantity"": 200
            }";

            using var jsonDoc = JsonDocument.Parse(jsonString);
            var json = jsonDoc.RootElement;

            // Act
            var result = await _controller.UpdateBikeDepot(nonExistingId, json);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateBikeDepot_ReturnsBadRequest_WhenIdsAreDifferent()
        {
            // Arrange
            var bikeDepot = new BikeDepot
            {
                BikeId = 4,
                BikeName = "Bike 1",
                BikeDescription = "Description 1",
                Quantity = 150,
                DateOfStore = DateTime.Now.AddDays(-3),
                ManuDepSeriesId = 2
            };

            var differentBikeId = 5;

            var mockDb = new Mock<DbSet<BikeDepot>>();
            mockDb.Setup(db => db.FindAsync(bikeDepot.BikeId)).ReturnsAsync(bikeDepot);

            var mockDbContext = new Mock<ModelContext>();
            mockDbContext.Setup(context => context.BikeDepots).Returns(mockDb.Object);

            var controller = new BikeDepotController(mockDbContext.Object);

            var json = JsonSerializer.Serialize(new
            {
                BikeName = "Updated Bike Name",
                BikeDescription = "Updated Description",
                Quantity = 100,
                DateOfStore = DateTime.Now.AddDays(-1),
                ManuDepSeriesId = 3
            });

            var jsonElement = JsonDocument.Parse(json).RootElement;

            // Act
            var result = await controller.UpdateBikeDepot(differentBikeId, jsonElement);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

    }
}