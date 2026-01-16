using Library.Entities;

namespace Library.Interfaces
{
    public interface IEmprestimoRepository
    {
        Task AddAsync(Emprestimo emprestimo);
        Task UpdateAsync(Emprestimo emprestimo);
        Task<Emprestimo?> BuscarPorIdAsync(int id);
        Task<IEnumerable<Emprestimo>> ListarAtivosPorUsuarioAsync(int usuarioId);
        Task<IEnumerable<Emprestimo>> ListarHistoricoPorUsuarioAsync(int usuarioId);
        Task<bool> PossuiMultaPendenteAsync(int usuarioId);
        Task<bool> ExisteEmprestimoAtivoPorLivroAsync(int livroId);
        Task<IEnumerable<Emprestimo>> ListarTodosAtivosAsync();
    }
}