using System;
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

        public OrderControllerTest()
        {
            DbContextOptions<BlobContext> options;
            var builder = new DbContextOptionsBuilder<BlobContext>();
            builder.UseInMemoryDatabase("OCT");
            options = builder.Options;
            _blobContext = new BlobContext(options);


            _orderController = new OrderController(_blobContext);
        }

        [Fact]
        public void GetAllOrders()
        {
            // Arrange = Setup
            _blobContext.Order.Add(new Order()
            {
                Id = 1,
                CreatedAt = new DateTime(),
                CustomerId = 1,
                OrderedCustomerId = 1,
                StateId = 1
            });

            _blobContext.SaveChanges();

            // Act = Processing
            var res = _orderController.GetAllOrders();

            // Assert = Testing
            // res.Result for example OK(...)
            Assert.IsType<ActionResult<Order>>(res.Result);
        }
    }
}
