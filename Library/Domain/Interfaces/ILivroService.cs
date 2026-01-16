using Library.DTOs;
using Library.Services;


namespace Library.Interfaces
{
    public interface ILivroService
    {
        Task<IEnumerable<LivroDTO>> ListarLivrosAsync(string? titulo, string? isbn);
        Task<IEnumerable<LivroDTO>> ListarEmEstoqueAsync();
        Task<IEnumerable<LivroDTO>> ListarPorAutor(int autorId);
        Task<LivroDTO> BuscarPorIdAsync(int id);
        Task<LivroDTO> CriarAsync(CreateLivroDTO dto);
        Task AtualizarAsync(int id, LivroDTO dto);

        Task RemoverAsync(int id);
    }
}