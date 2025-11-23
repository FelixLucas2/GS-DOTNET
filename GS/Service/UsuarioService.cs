using GS.Models;
using GS.Repositorios;

namespace GS.Servicos;

public class UsuarioService
{
    private readonly UsuarioRepository _repoUsuario;
    private readonly CursoRepository _repoCurso;
    private readonly ServicoRecomendacao _recomendador;

    public UsuarioService(UsuarioRepository repoUsuario, CursoRepository repoCurso, ServicoRecomendacao recomendador)
    {
        _repoUsuario = repoUsuario;
        _repoCurso = repoCurso;
        _recomendador = recomendador;
    }

    public Task<(List<Usuario> Itens, int Total)> ListarPaginadoAsync(int pagina = 1, int pageSize = 20) =>
        _repoUsuario.ListarPaginadoAsync(pagina, pageSize);

    public Task<Usuario?> ObterPorIdAsync(Guid id) => _repoUsuario.ObterPorIdAsync(id);

    public Task<Usuario> CriarAsync(Usuario usuario) => _repoUsuario.CriarAsync(usuario);

    public Task AtualizarAsync(Usuario usuario) => _repoUsuario.AtualizarAsync(usuario);

    public Task RemoverAsync(Guid id) => _repoUsuario.RemoverAsync(id);

    public Task AdicionarCursoAsync(Guid usuarioId, Guid cursoId) => _repoUsuario.AdicionarCursoAsync(usuarioId, cursoId);

    public Task RemoverCursoAsync(Guid usuarioId, Guid cursoId) => _repoUsuario.RemoverCursoAsync(usuarioId, cursoId);

    public Task<List<Curso>> ObterRecomendacoesAsync(Guid usuarioId, int top = 5)
    {
        // Busca o usuário e delega ao recomendador simples
        return _repoUsuario.ObterPorIdAsync(usuarioId).ContinueWith(t =>
        {
            var usuario = t.Result;
            if (usuario == null) throw new KeyNotFoundException("Usuário não encontrado.");
            return _recomendador.ObterRecomendacoesAsync(usuario, top);
        }).Unwrap();
    }
}
