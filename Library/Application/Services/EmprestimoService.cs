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

    public EmprestimoService(IEmprestimoRepository emprestimoRepository, TimeProvider timeProvider, IUsuarioRepository usuarioRepository, ILivroRepository livroRepository)
    {
        _emprestimoRepository = emprestimoRepository;
        _timeProvider = timeProvider;
        _usuarioRepository = usuarioRepository;
        _livroRepository = livroRepository;
    }

    public async Task<EmprestimoDTO?> BuscarPorIdAsync(int id)
    {
        var emprestimo = await _emprestimoRepository.BuscarPorIdAsync(id);

        if (emprestimo == null) return null;

        return new EmprestimoDTO
        {
            Id = emprestimo.Id,
            UsuarioId = emprestimo.UsuarioId,
            LivroId = emprestimo.LivroId,
            DataEmprestimo = emprestimo.DataEmprestimo,
            DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao,
            Ativo = emprestimo.Ativo
        };
    }

    public async Task<bool> DevolverEmprestimoAsync(int emprestimoId)
    {
        // Recupera o empréstimo pelo ID
        var emprestimo = await _emprestimoRepository.BuscarPorIdAsync(emprestimoId);

        if (emprestimo == null) throw new Exception("Empréstimo não encontrado.");
        if (!emprestimo.Ativo) throw new Exception("Empréstimo já foi devolvido.");

        // Recupera o livro e devolve 1 ao estoque
        var livro = await _livroRepository.BuscarPorIdAsync(emprestimo.LivroId);
        if (livro != null)
        {
            livro.QuantidadeEstoque++;
            await _livroRepository.UpdateAsync(livro);
        }

        // Atualiza data de devolução, calcula multa e desativa o empréstimo
        var agora = _timeProvider.GetLocalNow().DateTime;
        emprestimo.DataDevolucaoReal = agora;
        emprestimo.ValorMulta = CalcularMulta(emprestimo.DataPrevistaDevolucao, emprestimo.DataDevolucaoReal.Value);
        emprestimo.Ativo = false;

        await _emprestimoRepository.UpdateAsync(emprestimo);

        return true;
    }

    public async Task<IEnumerable<EmprestimoDTO>> ListarEmprestimosAtivosPorUsuarioAsync(int usuarioId)
    {
        var emprestimos = await _emprestimoRepository.ListarAtivosPorUsuarioAsync(usuarioId);
        return emprestimos
            .Select(e => new EmprestimoDTO
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                LivroId = e.LivroId,
                DataEmprestimo = e.DataEmprestimo,
                DataPrevistaDevolucao = e.DataPrevistaDevolucao,
                Ativo = e.Ativo
            });
    }

    public async Task<IEnumerable<EmprestimoDTO>> ListarHistoricoEmprestimosPorUsuarioAsync(int usuarioId)
    {
        var emprestimos = await _emprestimoRepository.ListarHistoricoPorUsuarioAsync(usuarioId);
        return emprestimos
            .Select(e => new EmprestimoDTO
            {
                Id = e.Id,
                UsuarioId = e.UsuarioId,
                LivroId = e.LivroId,
                DataEmprestimo = e.DataEmprestimo,
                DataPrevistaDevolucao = e.DataPrevistaDevolucao,
                Ativo = e.Ativo
            });
    }

    public async Task<EmprestimoDTO> RealizarEmprestimoAsync(CreateEmprestimoDTO dto)
    {
        
        // 1. Validar Usuário
        var usuario = await _usuarioRepository.BuscarPorIdAsync(dto.UsuarioId);
        if (usuario == null)
            throw new Exception("Usuário não encontrado.");

        if (!usuario.Ativo)
            throw new Exception("Usuário inativo não pode realizar empréstimos.");

        // 1.1 Regra: multa pendente
        var possuiMultaPendente = await _emprestimoRepository.PossuiMultaPendenteAsync(dto.UsuarioId);

        if (possuiMultaPendente)
            throw new Exception("Usuário possui multa pendente.");

        // 1.2 Regra: Máximo de 3 empréstimos simultâneos
        var emprestimosAtivos = await _emprestimoRepository.ListarAtivosPorUsuarioAsync(dto.UsuarioId);
        if (emprestimosAtivos.Count() >= 3)
            throw new Exception("Usuário já atingiu o limite máximo de 3 empréstimos simultâneos.");

        // 2. Validar Livro e Estoque
        var livro = await _livroRepository.BuscarPorIdAsync(dto.LivroId);
        if (livro == null)
            throw new Exception("Livro não encontrado.");

        if (livro.QuantidadeEstoque <= 0)
            throw new Exception("Livro indisponível no estoque.");

        // 3. Atualizar Estoque (Decrementa 1 unidade)
        livro.QuantidadeEstoque--;
        await _livroRepository.UpdateAsync(livro);

        // 4. Criar a Entidade Empréstimo
        var agora = _timeProvider.GetLocalNow().DateTime;
        var emprestimo = new Emprestimo
        {
            UsuarioId = dto.UsuarioId,
            Usuario = usuario, // Associa a entidade rastreada
            LivroId = dto.LivroId,
            Livro = livro,     // Associa a entidade rastreada
            DataEmprestimo = agora,
            DataPrevistaDevolucao = agora.AddDays(14), // Regra: 14 dias para devolução
            Ativo = true
        };

        await _emprestimoRepository.AddAsync(emprestimo);

        // 5. Retornar DTO
        return new EmprestimoDTO
        {
            Id = emprestimo.Id,
            UsuarioId = emprestimo.UsuarioId,
            LivroId = emprestimo.LivroId,
            DataEmprestimo = emprestimo.DataEmprestimo,
            DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao,
            Ativo = emprestimo.Ativo
        };
    }

    public async Task<bool> RenovarEmprestimoAsync(int emprestimoId)
    {
        var emprestimo = await _emprestimoRepository.BuscarPorIdAsync(emprestimoId);

        if (emprestimo == null) throw new Exception("Emprestimo não encontrado.");
        if (!emprestimo.Ativo) throw new Exception("Emprestimo já foi devolvido.");
        if (emprestimo.Renovado) throw new Exception("Emprestimo já foi renovado.");

        var agora = _timeProvider.GetLocalNow().DateTime;
        if (agora > emprestimo.DataPrevistaDevolucao) throw new Exception("Emprestimo já está atrasado.");
        
        

        emprestimo.DataPrevistaDevolucao = emprestimo.DataPrevistaDevolucao.AddDays(14);
        emprestimo.Renovado = true;

        await _emprestimoRepository.UpdateAsync(emprestimo);
        return true;
    }

    private decimal CalcularMulta(DateTime dataPrevista, DateTime dataReal)
    {
        if (dataReal.Date > dataPrevista.Date)
        {
            int diasAtraso = (dataReal.Date - dataPrevista.Date).Days;
            return diasAtraso * 2.0m;
        }
        return 0;
    }
}