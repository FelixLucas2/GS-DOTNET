using GS.Models;
using GS.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GS.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cursos")]
public class CursosController : ControllerBase
{
    private readonly CursoService _service;
    public CursosController(CursoService service) => _service = service;

    [HttpGet]
    public async Task<IActionResult> Listar([FromQuery] int pagina = 1, [FromQuery] int pageSize = 20)
    {
        var (itens, total) = await _service.ListarPaginadoAsync(pagina, pageSize);

        var data = itens.Select(c => new {
            c.Id,
            c.Titulo,
            c.Fornecedor,
            c.DuracaoMinutos,
            links = new[] {
                new { rel = "self", href = Url.Action(nameof(Obter), new { id = c.Id, version = "1.0" }) },
                new { rel = "update", href = Url.Action(nameof(Atualizar), new { id = c.Id, version = "1.0" }) },
                new { rel = "delete", href = Url.Action(nameof(Remover), new { id = c.Id, version = "1.0" }) }
            }
        });

        return Ok(new { meta = new { total, pagina, pageSize }, data });
    }

    // GET api/v1/cursos/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Obter(Guid id)
    {
        var c = await _service.ObterPorIdAsync(id);
        if (c == null) return NotFound();

        var response = new
        {
            c.Id,
            c.Titulo,
            c.Fornecedor,
            c.DuracaoMinutos,
            links = new[] {
                new { rel = "self", href = Url.Action(nameof(Obter), new { id = c.Id, version = "1.0" }) },
                new { rel = "update", href = Url.Action(nameof(Atualizar), new { id = c.Id, version = "1.0" }) },
                new { rel = "delete", href = Url.Action(nameof(Remover), new { id = c.Id, version = "1.0" }) }
            }
        };

        return Ok(response);
    }

    // POST api/v1/cursos
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] Curso curso)
    {
        var criado = await _service.CriarAsync(curso);

        var response = new
        {
            criado.Id,
            criado.Titulo,
            criado.Fornecedor,
            criado.DuracaoMinutos,
            links = new[] {
                new { rel = "self", href = Url.Action(nameof(Obter), new { id = criado.Id, version = "1.0" }) },
                new { rel = "update", href = Url.Action(nameof(Atualizar), new { id = criado.Id, version = "1.0" }) },
                new { rel = "delete", href = Url.Action(nameof(Remover), new { id = criado.Id, version = "1.0" }) }
            }
        };

        return CreatedAtAction(nameof(Obter), new { id = criado.Id, version = "1.0" }, response);
    }

    // PUT api/v1/cursos/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] Curso curso)
    {
        var existente = await _service.ObterPorIdAsync(id);
        if (existente == null) return NotFound();

        curso.Id = id;
        await _service.AtualizarAsync(curso);
        return NoContent();
    }

    // DELETE api/v1/cursos/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(Guid id)
    {
        var existente = await _service.ObterPorIdAsync(id);
        if (existente == null) return NotFound();

        await _service.RemoverAsync(id);
        return NoContent();
    }
}
