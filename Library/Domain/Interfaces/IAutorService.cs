using Library.DTOs;

namespace Library.Interfaces
{
    public interface IAutorService
    {
        Task<IEnumerable<AutorDto>> ListarAsync(int page, int size);
        Task<AutorDto?> BuscarPorIdAsync(int id);
        Task<AutorDto> CriarAsync(CreateAutorDto dto);
        Task<bool> AtualizarAsync(int id, CreateAutorDto dto);
        Task<bool> DesativarAsync(int id);
    }
}
