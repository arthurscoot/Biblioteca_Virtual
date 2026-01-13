using Library.Entities;

namespace Library.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<bool> ExisteCpfAsync(string cpf);
        Task<bool> ExisteEmailAsync(string email);

        Task AddAsync(Usuario usuario);

        Task<IEnumerable<Usuario>> ListarAtivosAsync();
        Task<Usuario?> BuscarPorCpfAsync(string cpf);
        Task<Usuario?> BuscarPorIdAsync(int id);

        Task<bool> ExisteCpfEmOutroUsuarioAsync(int id, string cpf);
        Task<bool> ExisteEmailEmOutroUsuarioAsync(int id, string email);

        Task UpdateAsync(Usuario usuario);
    }
}
