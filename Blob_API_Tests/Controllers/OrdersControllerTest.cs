using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blob_API.Controllers;
using Blob_API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Blob_API_Tests.Controllers
{
    // Use: AAA principle (Arrange, Act and Assert)
    public class OrdersControllerTest
    {
        // Arrange = Setup
        private readonly OrdersController _ordersController;
        private readonly BlobContext _blobContext;
        private Order newOrder = new Order()
        {
            Id = 1,
            CreatedAt = DateTime.Parse("2020-05-12 21:32:43"),
            CustomerId = 1,
            OrderedCustomerId = 1,
            StateId = 2
        };

        public OrdersControllerTest()
        {
            DbContextOptions<BlobContext> options;
            var builder = new DbContextOptionsBuilder<BlobContext>();
            builder.UseInMemoryDatabase("OCT");
            options = builder.Options;
            _blobContext = new BlobContext(options);

            SeedDatabase.SeedDatabaseWithDefaultData(_blobContext);

            _ordersController = new OrdersController(_blobContext, null);
        }

        [Fact]
        public async Task GetAllOrdersAsync()
        {
            // Act = Processing
            var task = await _ordersController.GetOrdersAsync();
            OkObjectResult result = task.Result as OkObjectResult;
            var list = result.Value as List<Order>;
            var testee = list.FirstOrDefault();

            // Assert
            Assert.Equal(newOrder.Id, testee.Id);
            Assert.Equal(newOrder.CreatedAt, testee.CreatedAt);
            Assert.Equal(newOrder.CustomerId, testee.CustomerId);
            Assert.Equal(newOrder.OrderedCustomerId, testee.OrderedCustomerId);
            Assert.Equal(newOrder.StateId, testee.StateId);
        }

        // Test put: two ojects one with id one without, check if the changes of the first are reverted!
    }
}
