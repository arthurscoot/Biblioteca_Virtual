using AutoMapper;
using Library.DTOs;
using Library.Entities;
using Library.Interfaces;
using Library.Services;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UsuarioService _service;

        public UsuarioServiceTests()
        {
            _mockRepository = new Mock<IUsuarioRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new UsuarioService(_mockRepository.Object, _mockMapper.Object);

            // Setup básico do Mapper
            _mockMapper.Setup(m => m.Map<Usuario>(It.IsAny<CreateUsuarioDTO>()))
                       .Returns((CreateUsuarioDTO d) => new Usuario { Nome = d.Nome, Cpf = d.Cpf, Email = d.Email });
            _mockMapper.Setup(m => m.Map<UsuarioDTO>(It.IsAny<Usuario>()))
                       .Returns((Usuario u) => new UsuarioDTO { Nome = u.Nome, Cpf = u.Cpf, Email = u.Email });
            _mockMapper.Setup(m => m.Map<IEnumerable<UsuarioDTO>>(It.IsAny<IEnumerable<Usuario>>()))
                       .Returns((IEnumerable<Usuario> src) => src.Select(u => new UsuarioDTO { Nome = u.Nome }));
        }

        [Fact]
        public async Task CriarAsync_DeveCriarUsuario_QuandoDadosValidos()
        {
            // Arrange
            var dto = new CreateUsuarioDTO { Nome = "Teste", Cpf = "12345678900", Email = "teste@email.com", Telefone = "123456789" };
            _mockRepository.Setup(r => r.ExisteCpfAsync(dto.Cpf)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.ExisteEmailAsync(dto.Email)).ReturnsAsync(false);

            // Act
            var result = await _service.CriarAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Nome, result.Nome);
            Assert.Equal(dto.Cpf, result.Cpf);
            Assert.Equal(dto.Email, result.Email);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoCpfJaExiste()
        {
            // Arrange
            var dto = new CreateUsuarioDTO { Nome = "Teste", Cpf = "12345678900", Email = "teste@email.com" };
            _mockRepository.Setup(r => r.ExisteCpfAsync(dto.Cpf)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
            Assert.Equal("Já existe um usuário cadastrado com este CPF.", ex.Message);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoEmailJaExiste()
        {
            // Arrange
            var dto = new CreateUsuarioDTO { Nome = "Teste", Cpf = "12345678900", Email = "teste@email.com" };
            _mockRepository.Setup(r => r.ExisteCpfAsync(dto.Cpf)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.ExisteEmailAsync(dto.Email)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CriarAsync(dto));
            Assert.Equal("Já existe um usuário cadastrado com este e-mail.", ex.Message);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task ListarAtivosAsync_DeveRetornarListaDeUsuarios()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Usuario { Id = 1, Nome = "User 1", Ativo = true },
                new Usuario { Id = 2, Nome = "User 2", Ativo = true }
            };
            _mockRepository.Setup(r => r.ListarAtivosAsync()).ReturnsAsync(usuarios);

            // Act
            var result = await _service.ListarAtivosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task BuscarPorCpfAsync_DeveRetornarUsuario_QuandoEncontrado()
        {
            // Arrange
            var cpf = "12345678900";
            var usuario = new Usuario { Id = 1, Nome = "Teste", Cpf = cpf, Ativo = true };
            _mockRepository.Setup(r => r.BuscarPorCpfAsync(cpf)).ReturnsAsync(usuario);

            // Act
            var result = await _service.BuscarPorCpfAsync(cpf);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cpf, result.Cpf);
        }

        [Fact]
        public async Task BuscarPorCpfAsync_DeveRetornarNull_QuandoNaoEncontrado()
        {
            // Arrange
            var cpf = "12345678900";
            _mockRepository.Setup(r => r.BuscarPorCpfAsync(cpf)).ReturnsAsync((Usuario?)null);

            // Act
            var result = await _service.BuscarPorCpfAsync(cpf);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarUsuario_QuandoDadosValidos()
        {
            // Arrange
            var id = 1;
            var dto = new CreateUsuarioDTO { Nome = "Novo Nome", Cpf = "12345678900", Email = "novo@email.com" };
            var usuario = new Usuario { Id = id, Nome = "Antigo", Cpf = "111", Email = "antigo@email.com", Ativo = true };

            _mockRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(usuario);
            _mockRepository.Setup(r => r.ExisteCpfEmOutroUsuarioAsync(id, dto.Cpf)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.ExisteEmailEmOutroUsuarioAsync(id, dto.Email)).ReturnsAsync(false);

            // Act
            var result = await _service.AtualizarAsync(id, dto);

            // Assert
            Assert.True(result);
            Assert.Equal(dto.Nome, usuario.Nome);
            Assert.Equal(dto.Cpf, usuario.Cpf);
            _mockRepository.Verify(r => r.UpdateAsync(usuario), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_DeveRetornarFalse_QuandoUsuarioNaoEncontrado()
        {
            // Arrange
            var id = 1;
            var dto = new CreateUsuarioDTO();
            _mockRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync((Usuario?)null);

            // Act
            var result = await _service.AtualizarAsync(id, dto);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoCpfEmUsoPorOutro()
        {
            // Arrange
            var id = 1;
            var dto = new CreateUsuarioDTO { Cpf = "12345678900" };
            var usuario = new Usuario { Id = id, Ativo = true };

            _mockRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(usuario);
            _mockRepository.Setup(r => r.ExisteCpfEmOutroUsuarioAsync(id, dto.Cpf)).ReturnsAsync(true);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AtualizarAsync(id, dto));
            Assert.Equal("O CPF informado já está em uso por outro usuário.", ex.Message);
        }

        [Fact]
        public async Task DesativarAsync_DeveDesativarUsuario_QuandoEncontrado()
        {
            // Arrange
            var id = 1;
            var usuario = new Usuario { Id = id, Ativo = true };
            _mockRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(usuario);

            // Act
            var result = await _service.DesativarAsync(id);

            // Assert
            Assert.True(result);
            Assert.False(usuario.Ativo);
            _mockRepository.Verify(r => r.UpdateAsync(usuario), Times.Once);
        }
    }
}