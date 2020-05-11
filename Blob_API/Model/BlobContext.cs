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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=176.31.26.11;database=Blob;user=remote_db;password=remote2251;sslmode=None;connection timeout=300;default command timeout=300", x => x.ServerVersion("10.4.12-mariadb"));
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
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
