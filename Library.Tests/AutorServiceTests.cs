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
                       .Returns((CreateAutorDto d) => new Autor(d.Nome, d.DataNascimento, d.PaisOrigem, d.Biografia));

            _mockMapper.Setup(m => m.Map<AutorDto>(It.IsAny<Autor>()))
                       .Returns((Autor a) => new AutorDto { Id = a.Id, Nome = a.Nome, PaisOrigem = a.PaisOrigem });

            _mockMapper.Setup(m => m.Map<IEnumerable<AutorDto>>(It.IsAny<IEnumerable<Autor>>()))
                       .Returns((IEnumerable<Autor> src) => src.Select(a => new AutorDto { Id = a.Id, Nome = a.Nome, PaisOrigem = a.PaisOrigem }));
        }

        private void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            prop?.SetValue(obj, value);
        }

        [Fact]
        public async Task ListarAsync_DeveRetornarListaDeAutores()
        {
            // Arrange
            var a1 = new Autor("Autor 1", DateTime.Now.AddYears(-20), "Brasil", "Bio");
            SetPrivateProperty(a1, "Id", 1);

            var a2 = new Autor("Autor 2", DateTime.Now.AddYears(-20), "EUA", "Bio");
            SetPrivateProperty(a2, "Id", 2);

            var autores = new List<Autor> { a1, a2 };
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
            var autor = new Autor("Autor Teste", DateTime.Now.AddYears(-20), "BR", "Bio");
            SetPrivateProperty(autor, "Id", 1);
            _mockRepository.Setup(r => r.BuscarAtivoPorIdAsync(1)).ReturnsAsync(autor);

            // Act
            var result = await _service.BuscarAtivoPorIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(autor.Nome, result.Nome);
        }

        [Fact]
        public async Task BuscarPorIdAsync_DeveLancarExcecao_QuandoNaoEncontrado()
        {
            // Arrange
            _mockRepository.Setup(r => r.BuscarAtivoPorIdAsync(1)).ReturnsAsync((Autor?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.BuscarAtivoPorIdAsync(1));
        }

        [Fact]
        public async Task BuscarPorIdAsync_DeveLancarExcecao_QuandoInativo()
        {
            // Arrange
            var autor = new Autor("Autor Inativo", DateTime.Now.AddYears(-20), "BR", "Bio");
            SetPrivateProperty(autor, "Id", 1);
            autor.Desativar();
            _mockRepository.Setup(r => r.BuscarAtivoPorIdAsync(1)).ReturnsAsync(autor);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _service.BuscarAtivoPorIdAsync(1));
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
            var dto = new CreateAutorDto { Nome = "Futuro", DataNascimento = DateTime.Today.AddDays(1), PaisOrigem = "BR" };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _service.CriarAsync(dto));
            Assert.Equal("A data de nascimento não pode ser futura.", ex.Message);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoMenorDe16Anos()
        {
            // Arrange
            var dto = new CreateAutorDto { Nome = "Jovem", DataNascimento = DateTime.Today.AddYears(-15), PaisOrigem = "BR" };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ValidationException>(() => _service.CriarAsync(dto));
            Assert.Equal("O autor deve ter no mínimo 16 anos.", ex.Message);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoNomeDuplicado()
        {
            // Arrange
            var dto = new CreateAutorDto { Nome = "Duplicado", DataNascimento = DateTime.Today.AddYears(-20) };
            _mockRepository.Setup(r => r.ExisteAutorComMesmoNomeAsync(dto.Nome)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.CriarAsync(dto));
            Assert.Equal("Já existe um autor com o mesmo nome.", ex.Message);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarAutor_QuandoDadosValidos()
        {
            // Arrange
            var id = 1;
            var dto = new CreateAutorDto { Nome = "Atualizado", DataNascimento = DateTime.Today.AddYears(-25), PaisOrigem = "Brasil" };
            var autor = new Autor("Antigo", DateTime.Now.AddYears(-20), "BR", "Bio");
            SetPrivateProperty(autor, "Id", id);

            _mockRepository.Setup(r => r.BuscarAtivoPorIdAsync(id)).ReturnsAsync(autor);

            // Act
            await _service.AtualizarAsync(id, dto);

            // Assert
            Assert.Equal(dto.Nome, autor.Nome);
            _mockRepository.Verify(r => r.UpdateAsync(autor), Times.Once);
        }

        [Fact]
        public async Task DesativarAsync_DeveDesativarAutor_QuandoEncontradoEAtivo()
        {
            // Arrange
            var id = 1;
            var autor = new Autor("Autor", DateTime.Now.AddYears(-20), "BR", "Bio");
            SetPrivateProperty(autor, "Id", id);
            _mockRepository.Setup(r => r.BuscarAtivoPorIdAsync(id)).ReturnsAsync(autor);

            // Act
            await _service.DesativarAsync(id);

            // Assert
            Assert.False(autor.Ativo);
            _mockRepository.Verify(r => r.UpdateAsync(autor), Times.Once);
        }
    }
}