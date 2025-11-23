using GS.Models;
using Microsoft.EntityFrameworkCore;

namespace GS.Repositorios;

public class UsuarioRepository
{
    private readonly AppDbContext _db;
    public UsuarioRepository(AppDbContext db) => _db = db;

    public async Task<(List<Usuario> Itens, int Total)> ListarPaginadoAsync(int pagina = 1, int pageSize = 20)
    {
        pagina = Math.Max(1, pagina);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _db.Usuarios.CountAsync();
        var itens = await _db.Usuarios
            .OrderBy(u => u.Nome)
            .Skip((pagina - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        return (itens, total);
    }

    public async Task<Usuario?> ObterPorIdAsync(Guid id) =>
        await _db.Usuarios.Include(u => u.Cursos).FirstOrDefaultAsync(u => u.Id == id);

    public async Task<Usuario> CriarAsync(Usuario usuario)
    {
        usuario.Id = usuario.Id == Guid.Empty ? Guid.NewGuid() : usuario.Id;
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();
        return usuario;
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        _db.Usuarios.Update(usuario);
        await _db.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var u = await _db.Usuarios.FindAsync(id);
        if (u == null) return;
        _db.Usuarios.Remove(u);
        await _db.SaveChangesAsync();
    }

    public async Task AdicionarCursoAsync(Guid usuarioId, Guid cursoId)
    {
        var usuario = await _db.Usuarios.Include(u => u.Cursos).FirstOrDefaultAsync(u => u.Id == usuarioId);
        if (usuario == null) throw new KeyNotFoundException("Usuário não encontrado.");
        var curso = await _db.Cursos.FindAsync(cursoId);
        if (curso == null) throw new KeyNotFoundException("Curso não encontrado.");
        if (!usuario.Cursos.Any(c => c.Id == cursoId))
        {
            usuario.Cursos.Add(curso);
            await _db.SaveChangesAsync();
        }
    }

    public async Task RemoverCursoAsync(Guid usuarioId, Guid cursoId)
    {
        var usuario = await _db.Usuarios.Include(u => u.Cursos).FirstOrDefaultAsync(u => u.Id == usuarioId);
        if (usuario == null) throw new KeyNotFoundException("Usuário não encontrado.");
        var curso = usuario.Cursos.FirstOrDefault(c => c.Id == cursoId);
        if (curso != null)
        {
            usuario.Cursos.Remove(curso);
            await _db.SaveChangesAsync();
        }
    }
}
