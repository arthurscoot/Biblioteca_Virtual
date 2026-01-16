using AutoMapper;
using Domain.Exceptions;
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

            _mockMapper.Setup(m => m.Map<UsuarioDTO>(It.IsAny<Usuario>()))
                       .Returns((Usuario u) => new UsuarioDTO { Nome = u.Nome, Cpf = u.Cpf, Email = u.Email });
            _mockMapper.Setup(m => m.Map<IEnumerable<UsuarioDTO>>(It.IsAny<IEnumerable<Usuario>>()))
                       .Returns((IEnumerable<Usuario> src) => src.Select(u => new UsuarioDTO { Nome = u.Nome }));
        }

        [Fact]
        public async Task CriarAsync_DeveCriarUsuario_QuandoDadosValidos()
        {
            var dto = new CreateUsuarioDTO 
            { 
                Nome = "Teste", 
                Cpf = "12345678900", 
                Email = "teste@email.com", 
                Telefone = "55 (11) 99999-9999",
                DataNascimento = new DateTime(2000, 1, 1),
                CpfResponsavel = null
            };
            _mockRepository.Setup(r => r.ExisteCpfAsync(dto.Cpf)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.ExisteEmailAsync(dto.Email)).ReturnsAsync(false);

            var result = await _service.CriarAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Nome, result.Nome);
            Assert.Equal(dto.Cpf, result.Cpf);
            Assert.Equal(dto.Email, result.Email);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoCpfJaExiste()
        {
            var dto = new CreateUsuarioDTO 
            { 
                Nome = "Teste", 
                Cpf = "12345678900", 
                Email = "teste@email.com",
                Telefone = "55 (11) 99999-9999",
                DataNascimento = new DateTime(2000, 1, 1)
            };
            _mockRepository.Setup(r => r.ExisteCpfAsync(dto.Cpf)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.CriarAsync(dto));
            Assert.Equal("Já existe um usuário cadastrado com este CPF.", ex.Message);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task CriarAsync_DeveLancarExcecao_QuandoEmailJaExiste()
        {
            var dto = new CreateUsuarioDTO 
            { 
                Nome = "Teste", 
                Cpf = "12345678900", 
                Email = "teste@email.com",
                Telefone = "55 (11) 99999-9999",
                DataNascimento = new DateTime(2000, 1, 1)
            };
            _mockRepository.Setup(r => r.ExisteCpfAsync(dto.Cpf)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.ExisteEmailAsync(dto.Email)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.CriarAsync(dto));
            Assert.Equal("Já existe um usuário cadastrado com este e-mail.", ex.Message);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task ListarAtivosAsync_DeveRetornarListaDeUsuarios()
        {
            var usuarios = new List<Usuario>
            {
                new Usuario("User 1", "11111111111", "email1@test.com", "5511999999999", new DateTime(2000, 1, 1), null),
                new Usuario("User 2", "22222222222", "email2@test.com", "5511999999999", new DateTime(2000, 1, 1), null)
            };
            _mockRepository.Setup(r => r.ListarAtivosAsync()).ReturnsAsync(usuarios);

            var result = await _service.ListarAtivosAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task BuscarPorCpfAsync_DeveRetornarUsuario_QuandoEncontrado()
        {
            var cpf = "12345678900";
            var usuario = new Usuario("Teste", cpf, "email@test.com", "5511999999999", new DateTime(2000, 1, 1), null);
            _mockRepository.Setup(r => r.BuscarPorCpfAsync(cpf)).ReturnsAsync(usuario);

            var result = await _service.BuscarPorCpfAsync(cpf);

            Assert.NotNull(result);
            Assert.Equal(cpf, result.Cpf);
        }

        [Fact]
        public async Task BuscarPorCpfAsync_DeveLancarExcecao_QuandoNaoEncontrado()
        {
            var cpf = "12345678900";
            _mockRepository.Setup(r => r.BuscarPorCpfAsync(cpf)).ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.BuscarPorCpfAsync(cpf));
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarUsuario_QuandoDadosValidos()
        {
            var id = 1;
            var dto = new CreateUsuarioDTO 
            { 
                Nome = "Novo Nome", 
                Cpf = "12345678900", 
                Email = "novo@email.com",
                Telefone = "55 (11) 88888-8888",
                DataNascimento = new DateTime(2000, 1, 1)
            };
            // Instanciando via construtor (Id será 0, mas o mock retorna este objeto quando solicitado)
            var usuario = new Usuario("Antigo", "111", "antigo@email.com", "5511999999999", new DateTime(1990, 1, 1), null);

            _mockRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(usuario);
            _mockRepository.Setup(r => r.ExisteCpfEmOutroUsuarioAsync(id, dto.Cpf)).ReturnsAsync(false);
            _mockRepository.Setup(r => r.ExisteEmailEmOutroUsuarioAsync(id, dto.Email)).ReturnsAsync(false);

            await _service.AtualizarAsync(id, dto);

            Assert.Equal(dto.Nome, usuario.Nome);
            Assert.Equal(dto.Cpf, usuario.Cpf);
            _mockRepository.Verify(r => r.UpdateAsync(usuario), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoUsuarioNaoEncontrado()
        {
            var id = 1;
            var dto = new CreateUsuarioDTO();
            _mockRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync((Usuario?)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.AtualizarAsync(id, dto));
            
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task AtualizarAsync_DeveLancarExcecao_QuandoCpfEmUsoPorOutro()
        {
            var id = 1;
            var dto = new CreateUsuarioDTO { Cpf = "12345678900" };
            var usuario = new Usuario("Teste", "111", "email@test.com", "5511999999999", new DateTime(2000, 1, 1), null);

            _mockRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(usuario);
            _mockRepository.Setup(r => r.ExisteCpfEmOutroUsuarioAsync(id, dto.Cpf)).ReturnsAsync(true);

            var ex = await Assert.ThrowsAsync<BusinessException>(() => _service.AtualizarAsync(id, dto));
            Assert.Equal("O CPF informado já está em uso por outro usuário.", ex.Message);
        }

        [Fact]
        public async Task DesativarAsync_DeveDesativarUsuario_QuandoEncontrado()
        {
            var id = 1;
            var usuario = new Usuario("Teste", "111", "email@test.com", "5511999999999", new DateTime(2000, 1, 1), null);
            _mockRepository.Setup(r => r.BuscarPorIdAsync(id)).ReturnsAsync(usuario);

            await _service.DesativarAsync(id);

            Assert.False(usuario.Ativo);
            _mockRepository.Verify(r => r.UpdateAsync(usuario), Times.Once);
        }
    }
}