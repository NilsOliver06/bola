using Microsoft.EntityFrameworkCore;
using LOGIN.Models;

namespace LOGIN.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoDetalle> PedidoDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Ciudad).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Edad).IsRequired();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuración de Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.ToTable("Productos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.Precio).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configuración de CarritoItem
            modelBuilder.Entity<CarritoItem>(entity =>
            {
                entity.ToTable("CarritoItems");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Pedido
            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.ToTable("Pedidos");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Usuario)
                    .WithMany()
                    .HasForeignKey(e => e.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Estado)
                    .HasConversion<int>();
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
                entity.Property(e => e.FechaPedido).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configuración de PedidoDetalle
            modelBuilder.Entity<PedidoDetalle>(entity =>
            {
                entity.ToTable("PedidoDetalles");
                entity.HasKey(e => e.Id);
                entity.HasOne(e => e.Pedido)
                    .WithMany(p => p.Detalles)
                    .HasForeignKey(e => e.PedidoId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Producto)
                    .WithMany()
                    .HasForeignKey(e => e.ProductoId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)");
            });
        }
    }
}