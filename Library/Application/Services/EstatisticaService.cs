
using AutoMapper;
using Library.Data.Repositories;
using Library.DTOs;
using Library.Interfaces;

public class EstatisticaService : IEstatisticaService
{
    private readonly IEmprestimoRepository _emprestimoRepository;
    private readonly IMapper _mapper;

    public EstatisticaService(IEmprestimoRepository emprestimoRepository, IMapper mapper)
    {
        _emprestimoRepository = emprestimoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TopAutorDTO>> ObterTopAutoresAsync()
    {
        var emprestimos = await _emprestimoRepository.ListarTodosAtivosAsync();

        return [.. emprestimos
            .GroupBy(e => e.Livro.Autor)
            .Select(g => new TopAutorDTO
            {
                Autor = _mapper.Map<AutorDto>(g.Key),
                QuantidadeEmprestimos = g.Count()
            })
            .OrderByDescending(x => x.QuantidadeEmprestimos)];
    }

    public async Task<IEnumerable<TopLivroDTO>> ObterTopLivrosAsync()
{
       var emprestimos = await _emprestimoRepository.ListarTodosAtivosAsync();

       return [.. emprestimos
            .GroupBy(e => e.Livro)
            .Select(g => new TopLivroDTO
            {
                Livro = _mapper.Map<LivroDTO>(g.Key),
                QuantidadeEmprestimos = g.Count()
            })
            .OrderByDescending(x => x.QuantidadeEmprestimos)];
}

}