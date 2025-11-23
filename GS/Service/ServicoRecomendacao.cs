using Microsoft.EntityFrameworkCore;
using GS.Models;

namespace GS.Servicos;

public class ServicoRecomendacao
{
    private readonly AppDbContext _db;
    public ServicoRecomendacao(AppDbContext db) => _db = db;

    public async Task<List<Curso>> ObterRecomendacoesAsync(Usuario usuario, int top = 5)
    {
        var skills = (usuario.Skills ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(s => s.Trim().ToLower()).ToList();
        var cursos = await _db.Cursos.ToListAsync();

        var ordenados = cursos.Select(c =>
        {
            var score = 0;
            var titulo = (c.Titulo ?? "").ToLower();
            foreach (var s in skills) if (!string.IsNullOrEmpty(s) && titulo.Contains(s)) score += 10;
            return new { Curso = c, Score = score };
        })
        .OrderByDescending(x => x.Score)
        .ThenBy(x => x.Curso.Titulo)
        .Take(top)
        .Select(x => x.Curso)
        .ToList();

        return ordenados;
    }
}
