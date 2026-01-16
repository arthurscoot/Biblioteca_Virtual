using AutoMapper;
using Domain.Exceptions;
using Library.DTOs;
using Library.Entities;
using Library.Interfaces;
using Library.Services;

namespace Library.Services;

public class EmprestimoService : IEmprestimoService
{
    private readonly IEmprestimoRepository _emprestimoRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILivroRepository _livroRepository;
    private readonly TimeProvider _timeProvider;
    private readonly IMapper _mapper;

    public EmprestimoService(IEmprestimoRepository emprestimoRepository, TimeProvider timeProvider, IUsuarioRepository usuarioRepository, ILivroRepository livroRepository, IMapper mapper)
    {
        _emprestimoRepository = emprestimoRepository;
        _timeProvider = timeProvider;
        _usuarioRepository = usuarioRepository;
        _livroRepository = livroRepository;
        _mapper = mapper;
    }

    public async Task<EmprestimoDTO> BuscarPorIdAsync(int id)
    {
        var emprestimo = await _emprestimoRepository.BuscarPorIdAsync(id);

        if (emprestimo == null) throw new NotFoundException("Empréstimo não encontrado.");

        return _mapper.Map<EmprestimoDTO>(emprestimo);
    }

    public async Task DevolverEmprestimoAsync(int emprestimoId)
    {
        var emprestimo = await _emprestimoRepository.BuscarPorIdAsync(emprestimoId);

        if (emprestimo == null) throw new NotFoundException("Empréstimo não encontrado.");

         emprestimo.Devolver(_timeProvider.GetLocalNow().DateTime);

        var livro = await _livroRepository.BuscarPorIdAsync(emprestimo.LivroId);
        if (livro != null)
        {
            livro.ReporEstoque();
            await _livroRepository.UpdateAsync(livro);
        }


        await _emprestimoRepository.UpdateAsync(emprestimo);
    }

    public async Task<IEnumerable<EmprestimoDTO>> ListarEmprestimosAtivosPorUsuarioAsync(int usuarioId)
    {
        var emprestimos = await _emprestimoRepository.ListarAtivosPorUsuarioAsync(usuarioId);
        return _mapper.Map<IEnumerable<EmprestimoDTO>>(emprestimos);
    }

    public async Task<IEnumerable<EmprestimoDTO>> ListarHistoricoEmprestimosPorUsuarioAsync(int usuarioId)
    {
        var emprestimos = await _emprestimoRepository.ListarHistoricoPorUsuarioAsync(usuarioId);
        return _mapper.Map<IEnumerable<EmprestimoDTO>>(emprestimos);
    }

    public async Task<EmprestimoDTO> RealizarEmprestimoAsync(CreateEmprestimoDTO dto)
    {
        var usuario = await _usuarioRepository.BuscarPorIdAsync(dto.UsuarioId);
        if (usuario == null)
            throw new NotFoundException("Usuário não encontrado.");

        if (!usuario.Ativo)
            throw new BusinessException("Usuário inativo não pode realizar empréstimos.");

        var possuiMultaPendente = await _emprestimoRepository.PossuiMultaPendenteAsync(dto.UsuarioId);

        if (possuiMultaPendente)
            throw new BusinessException("Usuário possui multa pendente.");

        var emprestimosAtivos = await _emprestimoRepository.ListarAtivosPorUsuarioAsync(dto.UsuarioId);
        if (emprestimosAtivos.Count() >= 3)
            throw new BusinessException("Usuário já atingiu o limite máximo de 3 empréstimos simultâneos.");

        var livro = await _livroRepository.BuscarPorIdAsync(dto.LivroId);
        if (livro == null)
            throw new NotFoundException("Livro não encontrado.");

        if (livro.QuantidadeEstoque <= 0)
            throw new BusinessException("Livro indisponível no estoque.");

        livro.BaixarEstoque();
        await _livroRepository.UpdateAsync(livro);

        var emprestimo = new Emprestimo(dto.UsuarioId, dto.LivroId, _timeProvider.GetLocalNow().DateTime);

        await _emprestimoRepository.AddAsync(emprestimo);

        return _mapper.Map<EmprestimoDTO>(emprestimo);
    }

    public async Task RenovarEmprestimoAsync(int emprestimoId)
    {
        var emprestimo = await _emprestimoRepository.BuscarPorIdAsync(emprestimoId);
        if (emprestimo == null) throw new NotFoundException("Empréstimo não encontrado.");

        emprestimo.Renovar(_timeProvider.GetLocalNow().DateTime);
        await _emprestimoRepository.UpdateAsync(emprestimo);
    }
}