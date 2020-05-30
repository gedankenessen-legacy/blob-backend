using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Blob_API.Model
{
    public partial class BlobContext : DbContext
    {
        public BlobContext()
        {
        }

        public BlobContext(DbContextOptions<BlobContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Address> Address { get; set; }
        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<CategoryProduct> CategoryProduct { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Location> Location { get; set; }
        public virtual DbSet<LocationProduct> LocationProduct { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderedAddress> OrderedAddress { get; set; }
        public virtual DbSet<OrderedCustomer> OrderedCustomer { get; set; }
        public virtual DbSet<OrderedProduct> OrderedProduct { get; set; }
        public virtual DbSet<OrderedProductOrder> OrderedProductOrder { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductProperty> ProductProperty { get; set; }
        public virtual DbSet<Property> Property { get; set; }
        public virtual DbSet<State> State { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("", x => x.ServerVersion("10.4.12-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Zip)
                    .IsRequired()
                    .HasColumnName("ZIP")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasData(
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
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("Name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasData(new Category()
                {
                    Id = 1,
                    Name = "Reifen"
                });
            });

            modelBuilder.Entity<CategoryProduct>(entity =>
            {
                entity.HasKey(e => new { e.CategoryId, e.ProductId })
                    .HasName("PRIMARY");

                entity.ToTable("Category_Product");

                entity.HasIndex(e => e.ProductId)
                    .HasName("FK_ProdCate");

                entity.Property(e => e.CategoryId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.ProductId).HasColumnType("int(11) unsigned");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategoryProduct)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cate");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.CategoryProduct)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProdCate");

                entity.HasData(
                    new CategoryProduct()
                    {
                        CategoryId = 1,
                        ProductId = 1
                    });
            });

            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasIndex(e => e.AddressId)
                    .HasName("FK_AddCust");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.AddressId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Customer)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddCust");

                entity.HasData(new Customer()
                {
                    Id = 1,
                    Firstname = "Philipp",
                    Lastname = "Heim",
                    CreatedAt = DateTime.Parse("2020-05-12 21:32:43"),
                    AddressId = 1
                });
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasIndex(e => e.AddressId)
                    .HasName("FK_AddLoc");

                entity.HasIndex(e => e.Name)
                    .HasName("Name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.AddressId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Location)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_AddLoc");

                entity.HasData(
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
            });

            modelBuilder.Entity<LocationProduct>(entity =>
            {
                entity.HasKey(e => new { e.LocationId, e.ProductId })
                    .HasName("PRIMARY");

                entity.ToTable("Location_Product");

                entity.HasIndex(e => e.ProductId)
                    .HasName("FK_ProdLoc");

                entity.Property(e => e.LocationId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.ProductId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.Quantity).HasColumnType("int(11) unsigned");

                entity.HasOne(d => d.Location)
                    .WithMany(p => p.LocationProduct)
                    .HasForeignKey(d => d.LocationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_LocProd");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.LocationProduct)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProdLoc");

                entity.HasData(
                    new LocationProduct()
                    {
                        LocationId = 1,
                        ProductId = 1,
                        Quantity = 8
                    });
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.CustomerId)
                    .HasName("FK_Cust");

                entity.HasIndex(e => e.OrderedCustomerId)
                    .HasName("FK_OrdCust");

                entity.HasIndex(e => e.StateId)
                    .HasName("FK_state");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.CustomerId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.OrderedCustomerId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.StateId).HasColumnType("int(11) unsigned");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Cust");

                entity.HasOne(d => d.OrderedCustomer)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.OrderedCustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrdCust");

                entity.HasOne(d => d.State)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.StateId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_state");

                entity.HasData(
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
            });

            modelBuilder.Entity<OrderedAddress>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.City)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Street)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Zip)
                    .IsRequired()
                    .HasColumnName("ZIP")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasData(
                    new OrderedAddress()
                    {
                        Id = 1,
                        Street = "Im Waldweg 3",
                        Zip = "77974",
                        City = "Meißenheim"
                    });
            });

            modelBuilder.Entity<OrderedCustomer>(entity =>
            {
                entity.HasIndex(e => e.OrderedAddressId)
                    .HasName("FK_OrdAdd");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.Firstname)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Lastname)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.OrderedAddressId).HasColumnType("int(11) unsigned");

                entity.HasOne(d => d.OrderedAddress)
                    .WithMany(p => p.OrderedCustomer)
                    .HasForeignKey(d => d.OrderedAddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrdAdd");

                entity.HasData(new OrderedCustomer()
                {
                    Id = 1,
                    Firstname = "Philipp",
                    Lastname = "Heim",
                    OrderedAddressId = 1
                });
            });

            modelBuilder.Entity<OrderedProduct>(entity =>
            {
                entity.HasIndex(e => e.ProductId)
                    .HasName("FK_ProdOrg");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Price).HasColumnType("decimal(10,0)");

                entity.Property(e => e.ProductId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.Sku)
                    .HasColumnName("SKU")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("'NO SKU DEFINED'")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderedProduct)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ProdOrg");

                entity.HasData(
                    new OrderedProduct() 
                    {
                        Id = 1,
                        Name = "Yokohama Sommerreifen",
                        Price = 250,
                        Sku = "110132751",
                        ProductId = 1
                    });
            });

            modelBuilder.Entity<OrderedProductOrder>(entity =>
            {
                entity.HasKey(e => new { e.OrderedProductId, e.OrderId })
                    .HasName("PRIMARY");

                entity.ToTable("OrderedProduct_Order");

                entity.HasIndex(e => e.OrderId)
                    .HasName("FK_Ord");

                entity.Property(e => e.OrderedProductId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.OrderId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.Quantity).HasColumnType("int(11) unsigned");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderedProductOrder)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ord");

                entity.HasOne(d => d.OrderedProduct)
                    .WithMany(p => p.OrderedProductOrder)
                    .HasForeignKey(d => d.OrderedProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrdPro");

                entity.HasData(
                    new OrderedProductOrder() 
                    { 
                        OrderedProductId = 1,
                        OrderId = 1,
                        Quantity = 4
                    });
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Price).HasColumnType("decimal(10,0)");

                entity.Property(e => e.Sku)
                    .HasColumnName("SKU")
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("'NO SKU DEFINED'")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasData(
                    new Product()
                    {
                        Id = 1,
                        Name = "Yokohama Sommerreifen",
                        Price = 250,
                        Sku = "110132751",
                        CreatedAt = DateTime.Parse("2020-05-12 21:32:43")
                    });
            });

            modelBuilder.Entity<ProductProperty>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.PropertyId })
                    .HasName("PRIMARY");

                entity.ToTable("Product_Property");

                entity.HasIndex(e => e.PropertyId)
                    .HasName("FK_Prop");

                entity.Property(e => e.ProductId).HasColumnType("int(11) unsigned");

                entity.Property(e => e.PropertyId).HasColumnType("int(11) unsigned");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductProperty)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prod");

                entity.HasOne(d => d.Property)
                    .WithMany(p => p.ProductProperty)
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Prop");

                entity.HasData(
                    new ProductProperty()
                    {
                        ProductId = 1,
                        PropertyId = 1
                    });
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("Name")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasData(
                    new Property()
                    {
                        Id = 1,
                        Name = "Größe",
                        Value = "205/50 R17"
                    });
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.HasIndex(e => e.Value)
                    .HasName("Value")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnType("varchar(255)")
                    .HasDefaultValueSql("''")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_general_ci");

                entity.HasData(
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
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
