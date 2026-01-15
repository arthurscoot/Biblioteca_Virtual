using Library.DTOs;

namespace Library.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> CriarAsync(CreateUsuarioDTO dto);
        Task<IEnumerable<UsuarioDTO>> ListarAtivosAsync();
        Task<UsuarioDTO> BuscarPorCpfAsync(string cpf);
        Task AtualizarAsync(int id, CreateUsuarioDTO dto);
        Task DesativarAsync(int id);
    }
}