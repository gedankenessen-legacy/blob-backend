using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Blob_API.Controllers;
using Blob_API.Model;
using Blob_API.RessourceModels;
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
        private readonly IMapper _mapper;
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

            MapperConfiguration mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new OrderProfile());
            });
            var mapper = mockMapper.CreateMapper();
            _mapper = mapper;

            SeedDatabase.SeedDatabaseWithDefaultData(_blobContext);

            _ordersController = new OrdersController(_blobContext, null, mapper);
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

        // Test PUT: two ojects one with id one without, check if the changes of the first are reverted!
        [Fact]
        public async Task InsertTwoButOnlyOneHasAId()
        {
            // Arrange = Setup
            List<Order> orders = new List<Order>();

            orders.Add(new Order()
            {
                Id = 1,
                CreatedAt = DateTime.Parse("2020-05-12 21:32:43"),
                CustomerId = 1,
                OrderedCustomerId = 1,
                StateId = 3 // 2->3
            });

            orders.Add(
            new Order()
            {
                Id = 0, // 2->0
                CreatedAt = DateTime.Parse("2020-05-12 23:56:08"),
                CustomerId = 1,
                OrderedCustomerId = 1,
                StateId = 4 // 1->4
            });


            // Act = Processing
            var task = await _ordersController.PutOrderAsync(_mapper.Map<IEnumerable<OrderRessource>>(orders));

            // Assert
            Assert.Equal((uint)2, _blobContext.Order.Find((uint)1).StateId);
            Assert.NotEqual((uint)3, _blobContext.Order.Find((uint)1).StateId);
            Assert.Equal((uint)1, _blobContext.Order.Find((uint)2).StateId);
            Assert.NotEqual((uint)4, _blobContext.Order.Find((uint)2).StateId);
        }
    }
}
