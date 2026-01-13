using Library.DTOs;

namespace Library.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioDTO> CriarAsync(CreateUsuarioDTO dto);
        Task<IEnumerable<UsuarioDTO>> ListarAtivosAsync();
        Task<UsuarioDTO?> BuscarPorCpfAsync(string cpf);
        Task<bool> AtualizarAsync(int id, CreateUsuarioDTO dto);
        Task<bool> DesativarAsync(int id);
    }
}