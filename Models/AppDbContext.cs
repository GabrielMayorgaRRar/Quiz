using Microsoft.EntityFrameworkCore;
using Quiz.Models;

public class AppDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Pregunta> Preguntas { get; set; } = null!;
    public DbSet<Categoria> Categorias { get; set; } = null!;
    public DbSet<Juego> Juegos { get; set; } = null!;

    public DbSet<Opciones> Opciones { get; set; } = null!;

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}
