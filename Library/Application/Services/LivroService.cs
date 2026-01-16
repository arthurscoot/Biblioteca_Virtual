using AutoMapper;
using Domain.Exceptions;
using Library.DTOs;
using Library.Entities;
using Library.Interfaces;
namespace Library.Services;

public class LivroService : ILivroService
{
    private readonly ILivroRepository _livroRepository;
    private readonly IAutorRepository _autorRepository;
    private readonly IEmprestimoRepository _emprestimoRepository;
    private readonly IMapper _mapper;

    public LivroService(ILivroRepository livroRepository, IAutorRepository autorRepository, IEmprestimoRepository emprestimoRepository, IMapper mapper)
    {
        _livroRepository = livroRepository;
        _autorRepository = autorRepository;
        _emprestimoRepository = emprestimoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LivroDTO>> ListarLivrosAsync(string? titulo, string? isbn)
    {
        var livros = await _livroRepository.ListarAsync(titulo, isbn);
        
        return _mapper.Map<IEnumerable<LivroDTO>>(livros);
    }
     public async Task<IEnumerable<LivroDTO>> ListarEmEstoqueAsync()
    {
        var livros = await _livroRepository.ListarEmEstoqueAsync();

        return _mapper.Map<IEnumerable<LivroDTO>>(livros);
    }

    public async Task<IEnumerable<LivroDTO>> ListarPorAutor(int autorId)
    {
        var autor = await _autorRepository.BuscarAtivoPorIdAsync(autorId);

        if (autor == null || !autor.Ativo)
        {
            return Enumerable.Empty<LivroDTO>();
        }

        var livros = await _livroRepository.ListarPorAutorAsync(autorId);
        
        return _mapper.Map<IEnumerable<LivroDTO>>(livros);
    }


     public async Task<LivroDTO> BuscarPorIdAsync(int id)
    {
        var livro = await _livroRepository.BuscarPorIdAsync(id);

        if (livro == null)
        {
            throw new NotFoundException("Livro não encontrado.");
        }

        return _mapper.Map<LivroDTO>(livro);
    }

    public async Task<LivroDTO?> BuscarPorISBNAsync(string isbn)
    {
        var livros = await _livroRepository.ListarAsync(null, isbn);
        var livro = livros.FirstOrDefault();
        return livro == null ? null : _mapper.Map<LivroDTO>(livro);
    }

    public async Task<LivroDTO?> BuscarPorNomeAsync(string nome)
    {
        var livros = await _livroRepository.ListarAsync(nome, null);
        var livro = livros.FirstOrDefault();
        return livro == null ? null : _mapper.Map<LivroDTO>(livro);
    }

    public async Task<LivroDTO> CriarAsync(CreateLivroDTO dto) {
        var autor = await _autorRepository.BuscarAtivoPorIdAsync(dto.AutorId);
        if (autor == null || !autor.Ativo)
        {
            throw new NotFoundException("Autor não encontrado ou inativo.");
        }

        var livroExistente = await _livroRepository.ExisteIsbnAsync(dto.ISBN);
        if (livroExistente)
        {
    
            throw new BusinessException("Já existe um livro com este ISBN.");
        }

        var novoLivro = new Livro(
            dto.Titulo,
            dto.ISBN,
            dto.AnoPublicacao,
            dto.Categoria,
            dto.QuantidadeEstoque,
            dto.AutorId
        );

        novoLivro.AssociarAutor(autor);

        await _livroRepository.AddAsync(novoLivro);

        return _mapper.Map<LivroDTO>(novoLivro);
    }
    
    public async Task AtualizarAsync(int id, LivroDTO dto)
    {
        var livro = await _livroRepository.BuscarPorIdAsync(id);

        if (livro == null)
            throw new NotFoundException("Livro não encontrado.");

        if (livro.AutorId != dto.AutorId)
        {
            var autor = await _autorRepository.BuscarAtivoPorIdAsync(dto.AutorId);
            if (autor == null || !autor.Ativo)
                throw new NotFoundException("Autor não encontrado ou inativo.");
        }

        if (livro.ISBN != dto.ISBN && await _livroRepository.ExisteIsbnEmOutroLivroAsync(id, dto.ISBN))
            throw new BusinessException("Já existe um livro com este ISBN.");

        livro.Atualizar(
            dto.Titulo,
            dto.ISBN,
            dto.AnoPublicacao,
            dto.Categoria,
            dto.QuantidadeEstoque,
            dto.AutorId
        );

        await _livroRepository.UpdateAsync(livro);
    }

    public async Task RemoverAsync(int id)
    {
        var livro = await _livroRepository.BuscarPorIdAsync(id);
        if (livro == null) throw new NotFoundException("Livro não encontrado.");

        if (await _emprestimoRepository.ExisteEmprestimoAtivoPorLivroAsync(id))
            throw new BusinessException("Não é possível remover o livro pois existem empréstimos ativos.");

        await _livroRepository.DeleteAsync(livro);
    }


    public async Task<IEnumerable<LivroDTO>> ListarTodosAsync(string titulo, string isbn)
    {
        var livros = await _livroRepository.ListarAsync(titulo, isbn);
        return _mapper.Map<IEnumerable<LivroDTO>>(livros);
    }
}