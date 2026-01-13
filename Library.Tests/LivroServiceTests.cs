using AutoMapper;
using Library.DTOs;
using Library.Entities;
using Library.Interfaces;
using Library.Services;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class LivroServiceTests
    {
        private readonly Mock<ILivroRepository> _mockLivroRepository;
        private readonly Mock<IAutorRepository> _mockAutorRepository;
        private readonly Mock<IEmprestimoRepository> _mockEmprestimoRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly LivroService _service;

        public LivroServiceTests()
        {
            _mockLivroRepository = new Mock<ILivroRepository>();
            _mockAutorRepository = new Mock<IAutorRepository>();
            _mockEmprestimoRepository = new Mock<IEmprestimoRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new LivroService(_mockLivroRepository.Object, _mockAutorRepository.Object, _mockEmprestimoRepository.Object, _mockMapper.Object);

            // Setup básico do Mapper para os testes não quebrarem com NullReference
            _mockMapper.Setup(m => m.Map<IEnumerable<LivroDTO>>(It.IsAny<IEnumerable<Livro>>()))
                       .Returns((IEnumerable<Livro> src) => src.Select(l => new LivroDTO { Id = l.Id, Titulo = l.Titulo }));
            _mockMapper.Setup(m => m.Map<LivroDTO>(It.IsAny<Livro>()))
                       .Returns((Livro l) => new LivroDTO { Id = l.Id, Titulo = l.Titulo });
            _mockMapper.Setup(m => m.Map<Livro>(It.IsAny<CreateLivroDTO>()))
                       .Returns((CreateLivroDTO d) => new Livro { Titulo = d.Titulo, ISBN = d.ISBN, AutorId = d.AutorId });
        }

        [Fact]
        public async Task ListarLivrosAsync_DeveRetornarLista_QuandoExistiremLivros()
        {
            // Arrange
            var livros = new List<Livro>
            {
                new Livro { Id = 1, Titulo = "Livro 1", Autor = new Autor { Nome = "Autor 1" } },
                new Livro { Id = 2, Titulo = "Livro 2", Autor = new Autor { Nome = "Autor 2" } }
            };
            _mockLivroRepository.Setup(r => r.ListarAsync(null, null)).ReturnsAsync(livros);

            // Act
            var result = await _service.ListarLivrosAsync(null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task ListarEmEstoqueAsync_DeveRetornarLivrosComEstoque()
        {
            // Arrange
            var livros = new List<Livro>
            {
                new Livro { Id = 1, Titulo = "Livro 1", QuantidadeEstoque = 5, Autor = new Autor { Nome = "Autor 1" } }
            };
            _mockLivroRepository.Setup(r => r.ListarEmEstoqueAsync()).ReturnsAsync(livros);

            // Act
            var result = await _service.ListarEmEstoqueAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().Id);
        }

        [Fact]
        public async Task ListarPorAutor_DeveRetornarVazio_QuandoAutorNaoExisteOuInativo()
        {
            // Arrange
            int autorId = 1;
            _mockAutorRepository.Setup(r => r.BuscarPorIdAsync(autorId)).ReturnsAsync((Autor?)null);

            // Act
            var result = await _service.ListarPorAutor(autorId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ListarPorAutor_DeveRetornarLivros_QuandoAutorExiste()
        {
            // Arrange
            int autorId = 1;
            var autor = new Autor { Id = autorId, Ativo = true, Nome = "Autor Teste" };
            var livros = new List<Livro>
            {
                new Livro { Id = 1, Titulo = "Livro 1", AutorId = autorId, Autor = autor }
            };

            _mockAutorRepository.Setup(r => r.BuscarPorIdAsync(autorId)).ReturnsAsync(autor);
            _mockLivroRepository.Setup(r => r.ListarPorAutorAsync(autorId)).ReturnsAsync(livros);

            // Act
            var result = await _service.ListarPorAutor(autorId);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task BuscarPorIdAsync_DeveRetornarLivro_QuandoEncontrado()
        {
            // Arrange
            int id = 1;
            var livro = new Livro { Id = id, Titulo = "Teste", Autor = new Autor { Nome = "Autor" } };
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(livro);

            // Act
            var result = await _service.BuscarPorIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task BuscarPorIdAsync_DeveRetornarNull_QuandoNaoEncontrado()
        {
            // Arrange
            int id = 1;
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync((Livro?)null);

            // Act
            var result = await _service.BuscarPorIdAsync(id);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CriarAsync_DeveCriarLivro_QuandoDadosValidos()
        {
            // Arrange
            var dto = new CreateLivroDTO { Titulo = "Novo Livro", ISBN = "123", AutorId = 1 };
            var autor = new Autor { Id = 1, Nome = "Autor", Ativo = true };

            _mockAutorRepository.Setup(r => r.BuscarPorIdAsync(dto.AutorId)).ReturnsAsync(autor);
            _mockLivroRepository.Setup(r => r.ExisteIsbnAsync(dto.ISBN)).ReturnsAsync(false);

            // Act
            var result = await _service.CriarAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Titulo, result.Titulo);
            _mockLivroRepository.Verify(r => r.AddAsync(It.IsAny<Livro>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoAutorInvalido()
        {
            // Arrange
            var dto = new CreateLivroDTO { AutorId = 1 };
            _mockAutorRepository.Setup(r => r.BuscarPorIdAsync(dto.AutorId)).ReturnsAsync((Autor?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.CriarAsync(dto));
            Assert.Equal("Autor não encontrado ou inativo.", ex.Message);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoIsbnJaExiste()
        {
            // Arrange
            var dto = new CreateLivroDTO { ISBN = "123", AutorId = 1 };
            var autor = new Autor { Id = 1, Ativo = true };

            _mockAutorRepository.Setup(r => r.BuscarPorIdAsync(dto.AutorId)).ReturnsAsync(autor);
            _mockLivroRepository.Setup(r => r.ExisteIsbnAsync(dto.ISBN)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
            Assert.Equal("Já existe um livro com este ISBN.", ex.Message);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoIsbnDuplicado()
        {
            // Arrange
            int id = 1;
            var dto = new LivroDTO { Id = id, ISBN = "999", AutorId = 1 }; // Changed ISBN
            var livro = new Livro { Id = id, ISBN = "123", AutorId = 1 };

            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(livro);
            _mockLivroRepository.Setup(r => r.ExisteIsbnEmOutroLivroAsync(id, dto.ISBN)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtualizarAsync(id, dto));
            Assert.Equal("Já existe um livro com este ISBN.", ex.Message);
        }

        [Fact]
        public async Task RemoverAsync_DeveRemoverLivro_QuandoNaoHouverEmprestimos()
        {
            // Arrange
            int id = 1;
            var livro = new Livro { Id = id };
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(livro);
            _mockEmprestimoRepository.Setup(r => r.ExisteEmprestimoAtivoPorLivroAsync(id)).ReturnsAsync(false);

            // Act
            var result = await _service.RemoverAsync(id);

            // Assert
            Assert.True(result);
            _mockLivroRepository.Verify(r => r.DeleteAsync(livro), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_DeveLancarExcecao_QuandoHouverEmprestimosAtivos()
        {
            // Arrange
            int id = 1;
            var livro = new Livro { Id = id };
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(livro);
            _mockEmprestimoRepository.Setup(r => r.ExisteEmprestimoAtivoPorLivroAsync(id)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.RemoverAsync(id));
            Assert.Equal("Não é possível remover o livro pois existem empréstimos ativos.", ex.Message);
            _mockLivroRepository.Verify(r => r.DeleteAsync(It.IsAny<Livro>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarLivro_QuandoDadosValidos()
        {
            // Arrange
            int id = 1;
            var dto = new LivroDTO { Id = id, Titulo = "Titulo Atualizado", ISBN = "1234567890123", AutorId = 1, AnoPublicacao = 2024, Categoria = "TI", QuantidadeEstoque = 10 };
            var livro = new Livro { Id = id, Titulo = "Antigo", ISBN = "1234567890123", AutorId = 1 };
            var autor = new Autor { Id = 1, Ativo = true };

            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(livro);
            _mockAutorRepository.Setup(r => r.BuscarPorIdAsync(dto.AutorId)).ReturnsAsync(autor);
            _mockLivroRepository.Setup(r => r.ExisteIsbnEmOutroLivroAsync(id, dto.ISBN)).ReturnsAsync(false);

            // Act
            var result = await _service.AtualizarAsync(id, dto);

            // Assert
            Assert.True(result);
            Assert.Equal("Titulo Atualizado", livro.Titulo);
            _mockLivroRepository.Verify(r => r.UpdateAsync(livro), Times.Once);
        }
    }
}