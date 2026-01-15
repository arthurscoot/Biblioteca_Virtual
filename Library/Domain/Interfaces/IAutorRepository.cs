using Library.Entities;

namespace Library.Interfaces
{
    public interface IAutorRepository
    {
        Task AddAsync(Autor autor);

        Task<IEnumerable<Autor>> ListarPorPageTamanhoAsync(int page, int size);
        Task<Autor?> BuscarAtivoPorIdAsync(int id);
        Task<bool> ExisteAutorComMesmoNomeAsync(string nome);

        Task UpdateAsync(Autor autor);
    }
}
