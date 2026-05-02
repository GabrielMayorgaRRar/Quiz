using Microsoft.EntityFrameworkCore;

namespace Quiz.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public AppDbContext()
    {
    }

    // DbSets - todas tus tablas
    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Pregunta> Preguntas { get; set; }
    public DbSet<Opciones> Opciones { get; set; }
    public DbSet<Juego> Juegos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Partida> Partidas { get; set; }
    public DbSet<DetallePartida> DetallesPartida { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relaciones
        modelBuilder.Entity<Partida>()
            .HasOne(p => p.Juego)
            .WithMany(j => j.Partidas)
            .HasForeignKey(p => p.JuegoId);

        modelBuilder.Entity<Partida>()
            .HasOne(p => p.Usuario)
            .WithMany(u => u.Partidas)
            .HasForeignKey(p => p.UsuarioId);

        modelBuilder.Entity<DetallePartida>()
            .HasOne(d => d.Partida)
            .WithMany(p => p.Detalles)
            .HasForeignKey(d => d.PartidaId);
    }
}