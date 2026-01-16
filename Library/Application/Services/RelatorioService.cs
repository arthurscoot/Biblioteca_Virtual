using AutoMapper;
using Library.DTOs;
using Library.Interfaces;

namespace Library.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IEmprestimoRepository _emprestimoRepository;
        private readonly IMapper _mapper;

        public RelatorioService(IEmprestimoRepository emprestimoRepository, IMapper mapper)
        {
            _emprestimoRepository = emprestimoRepository;
            _mapper = mapper;
        }
        public async Task<decimal> ObterTotalMultasAReceberAsync()
        {
            var emprestimos = await _emprestimoRepository.ListarComMultasPendentesAsync();
            return emprestimos.Sum(e => e.ValorMulta - e.ValorMultaPaga);
        }

        public async Task<IEnumerable<UsuarioDTO>> ObterUsuariosComEmprestimosAtrasadosAsync()
        {
            var usuarios = await _emprestimoRepository.ListarUsuariosComEmprestimosAtrasadosAsync();
            return _mapper.Map<IEnumerable<UsuarioDTO>>(usuarios);
        }
    }
}
