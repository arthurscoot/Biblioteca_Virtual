using System;

namespace Library.Entities
{
    public class Usuario {

    public int Id { get; set; }

    public String Nome { get; set; } = string.Empty;

    public String Cpf { get; set; } = string.Empty;

    public String Email { get; set; } = string.Empty;

    public String Telefone { get; set; } = string.Empty;

    public DateTime DatadeCadastro { get; set; }

    public bool Ativo { get; set; } = true;

    }
}