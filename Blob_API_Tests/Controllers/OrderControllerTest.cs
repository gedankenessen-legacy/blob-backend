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
    public class OrderControllerTest
    {
        // Arrange = Setup
        private readonly OrderController _orderController;
        private readonly BlobContext _blobContext;
        private Order newOrder = new Order()
        {
            Id = 1,
            CreatedAt = new DateTime(),
            CustomerId = 1,
            OrderedCustomerId = 1,
            StateId = 1
        };

        public OrderControllerTest()
        {
            DbContextOptions<BlobContext> options;
            var builder = new DbContextOptionsBuilder<BlobContext>();
            builder.UseInMemoryDatabase("OCT");
            options = builder.Options;
            _blobContext = new BlobContext(options);

            SeedDatabase();


            _orderController = new OrderController(_blobContext);
        }

        [Fact]
        public async Task GetAllOrdersAsync()
        {
            // Act = Processing
            var res = await _orderController.GetAllOrdersAsync();
            List<Order> value = res.Value.ToList();

            //Console.WriteLine(value);

            // Assert = Testing
            // Assert.IsType<Task<ActionResult<IEnumerable<Order>>>>(res);
            Assert.NotEqual(newOrder, value.First());
        }

        private void SeedDatabase()
        {
            // Arrange = Setup
            _blobContext.Order.Add(newOrder);
            _blobContext.SaveChanges();
        }
    }
}
