using Library.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Interfaces
{
    public interface IEmprestimoService
    {
        Task<EmprestimoDTO> RealizarEmprestimoAsync(CreateEmprestimoDTO dto);

        Task DevolverEmprestimoAsync(int emprestimoId);

        Task RenovarEmprestimoAsync(int emprestimoId);

        Task<IEnumerable<EmprestimoDTO>> ListarEmprestimosAtivosPorUsuarioAsync(int usuarioId);

        Task<IEnumerable<EmprestimoDTO>> ListarHistoricoEmprestimosPorUsuarioAsync(int usuarioId);

        Task<EmprestimoDTO> BuscarPorIdAsync(int id);
    }
}