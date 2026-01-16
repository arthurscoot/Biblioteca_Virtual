using Library.DTOs;
using Library.Entities;

public class TopAutorDTO
{
    public required AutorDto Autor { get; set; }
    public int QuantidadeEmprestimos { get; set; }
}