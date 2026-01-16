using Library.DTOs;

public class TopLivroDTO
{
    public required LivroDTO Livro { get; set; }
    public int QuantidadeEmprestimos { get; set; }
}