using Inventario.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using InventarioEntity = Inventario.Domain.Entities.Inventario;

public class InventarioDbContext : DbContext
{
    public InventarioDbContext(DbContextOptions<InventarioDbContext> options) : base(options) { }

    public DbSet<InventarioEntity> Inventarios { get; set; }
    public DbSet<CompraHistorico> ComprasHistorico { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InventarioEntity>().ToTable("Inventarios");

        modelBuilder.Entity<InventarioEntity>()
            .HasIndex(i => i.ProductoId)
            .IsUnique();

        modelBuilder.Entity<CompraHistorico>().ToTable("ComprasHistorico");

    }

}
