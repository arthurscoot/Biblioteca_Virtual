using AutoMapper;
using Domain.Exceptions;
using Library.DTOs;
using Library.Entities;
using Library.Interfaces;
using Library.Services;
using Moq;
using System.Reflection;
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
                       .Returns((CreateLivroDTO d) => new Livro(d.Titulo, d.ISBN, d.AnoPublicacao, d.Categoria, d.QuantidadeEstoque, d.AutorId));
        }

        private void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            prop?.SetValue(obj, value);
        }

        [Fact]
        public async Task ListarLivrosAsync_DeveRetornarLista_QuandoExistiremLivros()
        {
            // Arrange
            var autor1 = new Autor("Autor 1", DateTime.Now.AddYears(-20), "BR", "Bio");
            var autor2 = new Autor("Autor 2", DateTime.Now.AddYears(-20), "US", "Bio");

            var l1 = new Livro("Livro 1", "1234567890", 2020, "Cat", 10, 1);
            SetPrivateProperty(l1, "Id", 1);
            l1.AssociarAutor(autor1);

            var l2 = new Livro("Livro 2", "0987654321", 2021, "Cat", 10, 2);
            SetPrivateProperty(l2, "Id", 2);
            l2.AssociarAutor(autor2);

            var livros = new List<Livro>
            { l1, l2 };
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
            var l1 = new Livro("Livro 1", "1234567890", 2020, "Cat", 5, 1);
            SetPrivateProperty(l1, "Id", 1);
            
            var livros = new List<Livro> { l1 };
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
            _mockAutorRepository.Setup(r => r.BuscarAtivoPorIdAsync(autorId)).ReturnsAsync((Autor?)null);

            // Act & Assert
            // O serviço lança NotFoundException quando o autor não existe ou está inativo
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.ListarPorAutor(autorId));
            Assert.Equal("Autor não encontrado ou inativo.", ex.Message);
        }

        [Fact]
        public async Task ListarPorAutor_DeveRetornarLivros_QuandoAutorExiste()
        {
            // Arrange
            int autorId = 1;
            var autor = new Autor("Autor Teste", DateTime.Now.AddYears(-20), "BR", "Bio");
            SetPrivateProperty(autor, "Id", autorId);
            
            var l1 = new Livro("Livro 1", "1234567890", 2020, "Cat", 5, autorId);
            l1.AssociarAutor(autor);
            var livros = new List<Livro> { l1 };

            _mockAutorRepository.Setup(r => r.BuscarAtivoPorIdAsync(autorId)).ReturnsAsync(autor);
            _mockLivroRepository.Setup(r => r.ListarPorAutorAsync(autorId)).ReturnsAsync(livros);

            // Act
            var result = await _service.ListarPorAutor(autorId);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task BuscarAtivoPorIdAsync_DeveRetornarLivro_QuandoEncontrado()
        {
            // Arrange
            int id = 1;
            var livro = new Livro("Teste", "1234567890", 2020, "Cat", 5, 1);
            SetPrivateProperty(livro, "Id", id);
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(livro);

            // Act
            var result = await _service.BuscarPorIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task BuscarAtivoPorIdAsync_DeveLancarExcecao_QuandoNaoEncontrado()
        {
            // Arrange
            int id = 1;
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync((Livro?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.BuscarPorIdAsync(id));
        }

        [Fact]
        public async Task CriarAsync_DeveCriarLivro_QuandoDadosValidos()
        {
            // Arrange
            var dto = new CreateLivroDTO { Titulo = "Novo Livro", ISBN = "1234567890", AutorId = 1, Categoria = "Ficção", AnoPublicacao = 2024, QuantidadeEstoque = 5 };
            var autor = new Autor("Autor", DateTime.Now.AddYears(-20), "BR", "Bio");
            SetPrivateProperty(autor, "Id", 1);

            _mockAutorRepository.Setup(r => r.BuscarAtivoPorIdAsync(dto.AutorId)).ReturnsAsync(autor);
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
            _mockAutorRepository.Setup(r => r.BuscarAtivoPorIdAsync(dto.AutorId)).ReturnsAsync((Autor?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<NotFoundException>(() => _service.CriarAsync(dto));
            Assert.Equal("Autor não encontrado ou inativo.", ex.Message);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoIsbnJaExiste()
        {
            // Arrange
            var dto = new CreateLivroDTO { ISBN = "1234567890", AutorId = 1 };
            var autor = new Autor("Autor", DateTime.Now.AddYears(-20), "BR", "Bio");
            SetPrivateProperty(autor, "Id", 1);

            _mockAutorRepository.Setup(r => r.BuscarAtivoPorIdAsync(dto.AutorId)).ReturnsAsync(autor);
            _mockLivroRepository.Setup(r => r.ExisteIsbnAsync(dto.ISBN)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.CriarAsync(dto));
            Assert.Equal("Já existe um livro com este ISBN.", ex.Message);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoIsbnDuplicado()
        {
            // Arrange
            int id = 1;
            var dto = new LivroDTO { Id = id, ISBN = "9999999999", AutorId = 1 }; // Changed ISBN
            var livro = new Livro("Titulo", "1234567890", 2020, "Cat", 5, 1);
            SetPrivateProperty(livro, "Id", id);

            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(livro);
            _mockLivroRepository.Setup(r => r.ExisteIsbnEmOutroLivroAsync(id, dto.ISBN)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.AtualizarAsync(id, dto));
            Assert.Equal("Já existe um livro com este ISBN.", ex.Message);
        }

        [Fact]
        public async Task RemoverAsync_DeveRemoverLivro_QuandoNaoHouverEmprestimos()
        {
            // Arrange
            int id = 1;
            var livro = new Livro("Titulo", "1234567890", 2020, "Cat", 5, 1);
            SetPrivateProperty(livro, "Id", id);
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(livro);
            _mockEmprestimoRepository.Setup(r => r.ExisteEmprestimoAtivoPorLivroAsync(id)).ReturnsAsync(false);

            // Act
            await _service.RemoverAsync(id);

            // Assert
            _mockLivroRepository.Verify(r => r.DeleteAsync(livro), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_DeveLancarExcecao_QuandoHouverEmprestimosAtivos()
        {
            // Arrange
            int id = 1;
            var livro = new Livro("Titulo", "1234567890", 2020, "Cat", 5, 1);
            SetPrivateProperty(livro, "Id", id);
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(livro);
            _mockEmprestimoRepository.Setup(r => r.ExisteEmprestimoAtivoPorLivroAsync(id)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.RemoverAsync(id));
            Assert.Equal("Não é possível remover o livro pois existem empréstimos ativos.", ex.Message);
            _mockLivroRepository.Verify(r => r.DeleteAsync(It.IsAny<Livro>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarLivro_QuandoDadosValidos()
        {
            // Arrange
            int id = 1;
            var dto = new LivroDTO { Id = id, Titulo = "Titulo Atualizado", ISBN = "1234567890123", AutorId = 1, AnoPublicacao = 2024, Categoria = "TI", QuantidadeEstoque = 10 };
            var livro = new Livro("Antigo", "1234567890123", 2020, "Cat", 5, 1);
            SetPrivateProperty(livro, "Id", id);
            var autor = new Autor("Autor", DateTime.Now.AddYears(-20), "BR", "Bio");
            SetPrivateProperty(autor, "Id", 1);

            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(livro);
            _mockAutorRepository.Setup(r => r.BuscarAtivoPorIdAsync(dto.AutorId)).ReturnsAsync(autor);
            _mockLivroRepository.Setup(r => r.ExisteIsbnEmOutroLivroAsync(id, dto.ISBN)).ReturnsAsync(false);

            // Act
            await _service.AtualizarAsync(id, dto);

            // Assert
            Assert.Equal("Titulo Atualizado", livro.Titulo);
            _mockLivroRepository.Verify(r => r.UpdateAsync(livro), Times.Once);
        }
    }
}