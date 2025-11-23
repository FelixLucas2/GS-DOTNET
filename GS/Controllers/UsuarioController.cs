using GS.Models;
using GS.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GS.Controllers;

[Authorize] 
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/usuarios")]
public class UsuariosController : ControllerBase
{
    private readonly UsuarioService _service;
    public UsuariosController(UsuarioService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int pageSize = 20)
    {
        var (itens, total) = await _service.ListarPaginadoAsync(pagina, pageSize);
        var data = itens.Select(u => new {
            u.Id,
            u.Nome,
            u.Email,
            links = new[] { new { rel = "self", href = Url.Action(nameof(Obter), new { id = u.Id, version = "1.0" }) } }
        });
        return Ok(new { meta = new { total, pagina, pageSize }, data });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var u = await _service.ObterPorIdAsync(id);
        if (u == null) return NotFound();
        return Ok(u);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] Usuario usuario)
    {
        var criado = await _service.CriarAsync(usuario);
        return CreatedAtAction(nameof(Obter), new { id = criado.Id, version = "1.0" }, criado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] Usuario usuario)
    {
        var existente = await _service.ObterPorIdAsync(id);
        if (existente == null) return NotFound();
        usuario.Id = id;
        await _service.AtualizarAsync(usuario);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        var existente = await _service.ObterPorIdAsync(id);
        if (existente == null) return NotFound();
        await _service.RemoverAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/cursos/{cursoId}")]
    public async Task<IActionResult> AdicionarCurso(Guid id, Guid cursoId)
    {
        try
        {
            await _service.AdicionarCursoAsync(id, cursoId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}/cursos/{cursoId}")]
    public async Task<IActionResult> RemoverCurso(Guid id, Guid cursoId)
    {
        try
        {
            await _service.RemoverCursoAsync(id, cursoId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("{id}/recomendacoes")]
    public async Task<IActionResult> Recomendacoes(Guid id, [FromQuery] int top = 5)
    {
        try
        {
            var recs = await _service.ObterRecomendacoesAsync(id, top);
            var data = recs.Select(c => new { c.Id, c.Titulo, c.Fornecedor });
            return Ok(new { meta = new { usuario = id, count = recs.Count }, data });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
