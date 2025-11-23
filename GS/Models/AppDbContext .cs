using Microsoft.EntityFrameworkCore;
using GS.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> opts) : base(opts) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Curso> Cursos => Set<Curso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(b =>
        {
            b.HasKey(u => u.Id);
            b.Property(u => u.Email).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Curso>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Titulo).HasMaxLength(250).IsRequired();
        });

        // Many-to-many automática; EF cria tabela de junção
        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Cursos)
            .WithMany(c => c.Usuarios)
            .UsingEntity(j => j.ToTable("UsuarioCurso"));
    }
}
