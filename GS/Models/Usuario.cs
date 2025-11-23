using System;
using System.Collections.Generic;
using YamlDotNet.Core;

namespace GS.Models;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // relação muitos-para-muitos simples (EF Core gera tabela interna)
    public IList<Curso> Cursos { get; set; } = new List<Curso>();

    // skills apenas como texto simples (lista de strings serializada não é trivial sem 3ª entidade;
    // usaremos um campo único para simplificar — pode separar por vírgula)
    public string Skills { get; set; } = string.Empty; // ex: "python,sql,cloud"
}
