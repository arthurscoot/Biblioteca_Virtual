using AutoMapper;
using Library.DTOs;
using Library.Entities;
using Library.Interfaces;
using Library.Services;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class AutorServiceTests
    {
        private readonly Mock<IAutorRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AutorService _service;

        public AutorServiceTests()
        {
            _mockRepository = new Mock<IAutorRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new AutorService(_mockRepository.Object, _mockMapper.Object);

            // Setup básico do Mapper para evitar erros de referência nula nos testes
            _mockMapper.Setup(m => m.Map<Autor>(It.IsAny<CreateAutorDto>()))
                       .Returns((CreateAutorDto d) => new Autor { Nome = d.Nome, DataNascimento = d.DataNascimento, PaisOrigem = d.PaisOrigem, Biografia = d.Biografia });

            _mockMapper.Setup(m => m.Map<AutorDto>(It.IsAny<Autor>()))
                       .Returns((Autor a) => new AutorDto { Id = a.Id, Nome = a.Nome, PaisOrigem = a.PaisOrigem });

            _mockMapper.Setup(m => m.Map<IEnumerable<AutorDto>>(It.IsAny<IEnumerable<Autor>>()))
                       .Returns((IEnumerable<Autor> src) => src.Select(a => new AutorDto { Id = a.Id, Nome = a.Nome, PaisOrigem = a.PaisOrigem }));
        }

        [Fact]
        public async Task ListarAsync_DeveRetornarListaDeAutores()
        {
            // Arrange
            var autores = new List<Autor>
            {
                new Autor { Id = 1, Nome = "Autor 1", PaisOrigem = "Brasil", Ativo = true },
                new Autor { Id = 2, Nome = "Autor 2", PaisOrigem = "EUA", Ativo = true }
            };
            _mockRepository.Setup(r => r.ListarPorPageTamanhoAsync(1, 10)).ReturnsAsync(autores);

            // Act
            var result = await _service.ListarAsync(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task BuscarPorIdAsync_DeveRetornarAutor_QuandoEncontradoEAtivo()
        {
            // Arrange
            var autor = new Autor { Id = 1, Nome = "Autor Teste", Ativo = true };
            _mockRepository.Setup(r => r.BuscarPorIdAsync(1)).ReturnsAsync(autor);

            // Act
            var result = await _service.BuscarPorIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(autor.Nome, result.Nome);
        }

        [Fact]
        public async Task BuscarPorIdAsync_DeveRetornarNull_QuandoNaoEncontrado()
        {
            // Arrange
            _mockRepository.Setup(r => r.BuscarPorIdAsync(1)).ReturnsAsync((Autor?)null);

            // Act
            var result = await _service.BuscarPorIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task BuscarPorIdAsync_DeveRetornarNull_QuandoInativo()
        {
            // Arrange
            var autor = new Autor { Id = 1, Nome = "Autor Inativo", Ativo = false };
            _mockRepository.Setup(r => r.BuscarPorIdAsync(1)).ReturnsAsync(autor);

            // Act
            var result = await _service.BuscarPorIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CriarAsync_DeveCriarAutor_QuandoDadosValidos()
        {
            // Arrange
            var dto = new CreateAutorDto
            {
                Nome = "Novo Autor",
                DataNascimento = DateTime.Today.AddYears(-20),
                PaisOrigem = "Brasil",
                Biografia = "Bio"
            };
            _mockRepository.Setup(r => r.ExisteAutorComMesmoNomeAsync(dto.Nome)).ReturnsAsync(false);

            // Act
            var result = await _service.CriarAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Nome, result.Nome);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Autor>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoDataNascimentoNoFuturo()
        {
            // Arrange
            var dto = new CreateAutorDto { Nome = "Futuro", DataNascimento = DateTime.Today.AddDays(1) };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CriarAsync(dto));
            Assert.Equal("A data de nascimento não pode ser maior que a data atual.", ex.Message);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoMenorDe16Anos()
        {
            // Arrange
            var dto = new CreateAutorDto { Nome = "Jovem", DataNascimento = DateTime.Today.AddYears(-15) };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CriarAsync(dto));
            Assert.Equal("O autor deve ter no mínimo 16 anos.", ex.Message);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoNomeDuplicado()
        {
            // Arrange
            var dto = new CreateAutorDto { Nome = "Duplicado", DataNascimento = DateTime.Today.AddYears(-20) };
            _mockRepository.Setup(r => r.ExisteAutorComMesmoNomeAsync(dto.Nome)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CriarAsync(dto));
            Assert.Equal("Já existe um autor com o mesmo nome.", ex.Message);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarAutor_QuandoDadosValidos()
        {
            // Arrange
            var id = 1;
            var dto = new CreateAutorDto { Nome = "Atualizado", DataNascimento = DateTime.Today.AddYears(-25) };
            var autor = new Autor { Id = id, Nome = "Antigo", Ativo = true };

            _mockRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(autor);

            // Act
            var result = await _service.AtualizarAsync(id, dto);

            // Assert
            Assert.True(result);
            Assert.Equal(dto.Nome, autor.Nome);
            _mockRepository.Verify(r => r.UpdateAsync(autor), Times.Once);
        }

        [Fact]
        public async Task DesativarAsync_DeveDesativarAutor_QuandoEncontradoEAtivo()
        {
            // Arrange
            var id = 1;
            var autor = new Autor { Id = id, Ativo = true };
            _mockRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(autor);

            // Act
            var result = await _service.DesativarAsync(id);

            // Assert
            Assert.True(result);
            Assert.False(autor.Ativo);
            _mockRepository.Verify(r => r.UpdateAsync(autor), Times.Once);
        }
    }
}