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
        // 1. Verifica se o autor com o ID fornecido existe e está ativo.
        //    Isso evita buscar livros para um autor inválido ou inativo.
        var autor = await _autorRepository.BuscarAtivoPorIdAsync(autorId);

        if (autor == null || !autor.Ativo)
        {
            // Retorna uma lista vazia se o autor não for encontrado.
            return Enumerable.Empty<LivroDTO>();
        }

        // 2. Busca os livros do autor, já convertendo para DTO.
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
        // Reutiliza o método de listar filtrando por ISBN
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
        // 1. Chama o AutorService para validar se o autor existe e está ativo.
        var autor = await _autorRepository.BuscarAtivoPorIdAsync(dto.AutorId);
        if (autor == null || !autor.Ativo)
        {
            // Usar uma exceção mais específica é uma boa prática.
            throw new NotFoundException("Autor não encontrado ou inativo.");
        }

        // 2. Verifica se já existe um livro com o mesmo ISBN (comparando string com string).
        var livroExistente = await _livroRepository.ExisteIsbnAsync(dto.ISBN);
        if (livroExistente)
        {
    
            throw new BusinessException("Já existe um livro com este ISBN.");
        }

        // 3. Cria a nova entidade Livro
        var novoLivro = _mapper.Map<Livro>(dto);

        // 4. Salva no banco de dados
        await _livroRepository.AddAsync(novoLivro);

        novoLivro.Autor = autor;

        return _mapper.Map<LivroDTO>(novoLivro);
    }
    
    public async Task AtualizarAsync(int id, LivroDTO dto)
    {
        var livro = await _livroRepository.BuscarPorIdAsync(id);

        if (livro == null)
            throw new NotFoundException("Livro não encontrado.");

        // Verifica se o autor existe se o ID mudou
        if (livro.AutorId != dto.AutorId)
        {
            var autor = await _autorRepository.BuscarAtivoPorIdAsync(dto.AutorId);
            if (autor == null || !autor.Ativo)
                throw new NotFoundException("Autor não encontrado ou inativo.");
        }

        // Verifica se o ISBN já existe em outro livro
        if (livro.ISBN != dto.ISBN && await _livroRepository.ExisteIsbnEmOutroLivroAsync(id, dto.ISBN))
            throw new BusinessException("Já existe um livro com este ISBN.");

        livro.Titulo = dto.Titulo;
        livro.ISBN = dto.ISBN;
        livro.AnoPublicacao = dto.AnoPublicacao;
        livro.Categoria = dto.Categoria;
        livro.QuantidadeEstoque = dto.QuantidadeEstoque;
        livro.AutorId = dto.AutorId;

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