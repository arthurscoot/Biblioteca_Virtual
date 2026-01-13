using Library.Entities;

namespace Library.Interfaces
{
    public interface ILivroRepository
    {
        Task AddAsync(Livro livro);
        Task UpdateAsync(Livro livro);
        Task DeleteAsync(Livro livro);

        Task<Livro?> BuscarPorIdAsync(int id);
        Task<IEnumerable<Livro>> ListarAsync(string? titulo, string? isbn);
        Task<IEnumerable<Livro>> ListarEmEstoqueAsync();
        Task<IEnumerable<Livro>> ListarPorAutorAsync(int autorId);
        Task<bool> ExisteIsbnAsync(string isbn);
        Task<bool> ExisteIsbnEmOutroLivroAsync(int id, string isbn);
    }
}
