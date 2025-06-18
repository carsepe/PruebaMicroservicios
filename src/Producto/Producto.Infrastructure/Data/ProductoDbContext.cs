using Microsoft.EntityFrameworkCore;
using ProductoEntity = Producto.Domain.Entities.Producto;

namespace Producto.Infrastructure.Data
{
    public class ProductoDbContext : DbContext
    {
        public ProductoDbContext(DbContextOptions<ProductoDbContext> options) : base(options) { }

        public DbSet<ProductoEntity> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductoEntity>().ToTable("Productos");
        }
    }
}

