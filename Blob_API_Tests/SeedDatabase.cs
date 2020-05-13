using Blob_API.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blob_API_Tests
{
    public static class SeedDatabase
    {
        public static void SeedDatabaseWithDefaultData(BlobContext context)
        {
            context.Address.AddRange(
                new Address()
                {
                    Id = 1,
                    Street = "Im Waldweg 3",
                    Zip = "77974",
                    City = "Meißenheim"
                },
                new Address()
                {
                    Id = 2,
                    Street = "Königsgasse 14",
                    Zip = "77770",
                    City = "Durbach"
                });

            context.Category.Add(
                new Category()
                {
                    Id = 1,
                    Name = "Reifen"
                });

            context.CategoryProduct.Add(
                new CategoryProduct()
                {
                    CategoryId = 1,
                    ProductId = 1
                });

            context.Customer.Add(
                new Customer()
                {
                    Id = 1,
                    Firstname = "Philipp",
                    Lastname = "Heim",
                    CreatedAt = DateTime.Parse("2020-05-12 21:32:43"),
                    AddressId = 1
                });

            context.Location.AddRange(
                new Location()
                {
                    Id = 1,
                    Name = "Filiale 1",
                    AddressId = 1
                },
                new Location()
                {
                    Id = 2,
                    Name = "Filiale 2",
                    AddressId = 2
                });

            context.LocationProduct.Add(
                new LocationProduct()
                {
                    LocationId = 1,
                    ProductId = 1,
                    Quantity = 8
                });

            context.Order.AddRange(
                new Order()
                {
                    Id = 1,
                    CreatedAt = DateTime.Parse("2020-05-12 21:32:43"),
                    CustomerId = 1,
                    OrderedCustomerId = 1,
                    StateId = 2
                },
                new Order()
                {
                    Id = 2,
                    CreatedAt = DateTime.Parse("2020-05-12 23:56:08"),
                    CustomerId = 1,
                    OrderedCustomerId = 1,
                    StateId = 1
                });

            context.OrderedAddress.Add(
                new OrderedAddress()
                {
                    Id = 1,
                    Street = "Im Waldweg 3",
                    Zip = "77974",
                    City = "Meißenheim"
                });

            context.OrderedCustomer.Add(
                new OrderedCustomer()
                {
                    Id = 1,
                    Firstname = "Philipp",
                    Lastname = "Heim",
                    OrderedAddressId = 1
                });

            context.OrderedProduct.Add(
                new OrderedProduct()
                {
                    Id = 1,
                    Name = "Yokohama Sommerreifen",
                    Price = 250,
                    Sku = "110132751",
                    ProductId = 1
                });

            context.OrderedProductOrder.Add(
                new OrderedProductOrder()
                {
                    OrderedProductId = 1,
                    OrderId = 1,
                    Quantity = 4
                });

            context.Product.Add(
                new Product()
                {
                    Id = 1,
                    Name = "Yokohama Sommerreifen",
                    Price = 250,
                    Sku = "110132751",
                    CreatedAt = DateTime.Parse("2020-05-12 21:32:43")
                });

            context.ProductProperty.Add(
                new ProductProperty()
                {
                    ProductId = 1,
                    PropertyId = 1
                });

            context.Property.Add(
                new Property()
                {
                    Id = 1,
                    Name = "Größe",
                    Value = "205/50 R17"
                });

            context.State.AddRange(
                new State()
                {
                    Id = 1,
                    Value = "Erstellt"
                },
                new State()
                {
                    Id = 2,
                    Value = "In Bearbeitung"
                },
                new State()
                {
                    Id = 3,
                    Value = "Versand"
                },
                new State()
                {
                    Id = 4,
                    Value = "Archiviert"
                });

            context.SaveChanges();
        }
    }
}
