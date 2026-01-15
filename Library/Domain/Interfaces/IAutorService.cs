using Library.DTOs;

namespace Library.Interfaces
{
    public interface IAutorService
    {
        Task<IEnumerable<AutorDto>> ListarAsync(int page, int size);
        Task<AutorDto> BuscarAtivoPorIdAsync(int id);
        Task<AutorDto> CriarAsync(CreateAutorDto dto);
        Task AtualizarAsync(int id, CreateAutorDto dto);
        Task DesativarAsync(int id);
    }
}
