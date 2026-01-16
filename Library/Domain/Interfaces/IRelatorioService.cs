using Library.DTOs;

namespace Library.Interfaces
{
    public interface IRelatorioService
    {
        Task<decimal> ObterTotalMultasAReceberAsync();
        Task<IEnumerable<UsuarioDTO>> ObterUsuariosComEmprestimosAtrasadosAsync();
    }
}
