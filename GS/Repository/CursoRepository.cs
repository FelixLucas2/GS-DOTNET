using GS.Models;
using Microsoft.EntityFrameworkCore;

namespace GS.Repositorios;

public class CursoRepository
{
    private readonly AppDbContext _db;
    public CursoRepository(AppDbContext db) => _db = db;

    public async Task<(List<Curso> Itens, int Total)> ListarPaginadoAsync(int pagina = 1, int pageSize = 20)
    {
        pagina = Math.Max(1, pagina);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var total = await _db.Cursos.CountAsync();
        var itens = await _db.Cursos.OrderBy(c => c.Titulo)
            .Skip((pagina - 1) * pageSize).Take(pageSize).ToListAsync();
        return (itens, total);
    }

    public async Task<Curso?> ObterPorIdAsync(Guid id) =>
        await _db.Cursos.FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Curso> CriarAsync(Curso curso)
    {
        curso.Id = curso.Id == Guid.Empty ? Guid.NewGuid() : curso.Id;
        _db.Cursos.Add(curso);
        await _db.SaveChangesAsync();
        return curso;
    }

    public async Task AtualizarAsync(Curso curso)
    {
        _db.Cursos.Update(curso);
        await _db.SaveChangesAsync();
    }

    public async Task RemoverAsync(Guid id)
    {
        var c = await _db.Cursos.FindAsync(id);
        if (c == null) return;
        _db.Cursos.Remove(c);
        await _db.SaveChangesAsync();
    }
}
