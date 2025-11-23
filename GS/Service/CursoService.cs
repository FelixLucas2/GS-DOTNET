using GS.Models;
using GS.Repositorios;

namespace GS.Servicos;

public class CursoService
{
    private readonly CursoRepository _repo;
    public CursoService(CursoRepository repo) => _repo = repo;

    public Task<(List<Curso> Itens, int Total)> ListarPaginadoAsync(int pagina = 1, int pageSize = 20) =>
        _repo.ListarPaginadoAsync(pagina, pageSize);

    public Task<Curso?> ObterPorIdAsync(Guid id) => _repo.ObterPorIdAsync(id);

    public Task<Curso> CriarAsync(Curso curso) => _repo.CriarAsync(curso);

    public Task AtualizarAsync(Curso curso) => _repo.AtualizarAsync(curso);

    public Task RemoverAsync(Guid id) => _repo.RemoverAsync(id);
}
