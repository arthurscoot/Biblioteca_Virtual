using Library.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Library.Interfaces
{
    public interface IEmprestimoService
    {
        Task<EmprestimoDTO> RealizarEmprestimoAsync(CreateEmprestimoDTO dto);

        Task<bool> DevolverEmprestimoAsync(int emprestimoId);

        Task<bool> RenovarEmprestimoAsync(int emprestimoId);

        Task<IEnumerable<EmprestimoDTO>> ListarEmprestimosAtivosPorUsuarioAsync(int usuarioId);

        Task<IEnumerable<EmprestimoDTO>> ListarHistoricoEmprestimosPorUsuarioAsync(int usuarioId);
    }
}