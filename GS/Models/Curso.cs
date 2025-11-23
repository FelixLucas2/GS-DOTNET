using System;
using System.Collections.Generic;

namespace GS.Models;

public class Curso
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string Fornecedor { get; set; } = string.Empty;
    public int DuracaoMinutos { get; set; }

    // relação inversa (muitos-para-muitos)
    public IList<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
