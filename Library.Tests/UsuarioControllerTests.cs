using Library.Controllers;
using Library.DTOs;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class UsuarioControllerTests
    {
        private readonly Mock<IUsuarioService> _mockService;
        private readonly UsuarioController _controller;

        public UsuarioControllerTests()
        {
            _mockService = new Mock<IUsuarioService>();
            _controller = new UsuarioController(_mockService.Object);
        }

        [Fact]
        public async Task Criar_DeveRetornarCreated_QuandoSucesso()
        {
            // Arrange
            var dto = new CreateUsuarioDTO { Nome = "Novo Usuario", Cpf = "123" };
            var usuarioCriado = new UsuarioDTO { Nome = "Novo Usuario", Cpf = "123" };
            _mockService.Setup(s => s.CriarAsync(dto)).ReturnsAsync(usuarioCriado);

            // Act
            var result = await _controller.Criar(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(UsuarioController.BuscarPorCpf), createdResult.ActionName);
            Assert.Equal("123", createdResult.RouteValues?["cpf"]);
            Assert.Equal(usuarioCriado, createdResult.Value);
        }

        [Fact]
        public async Task ListarAtivos_DeveRetornarOk_ComLista()
        {
            // Arrange
            var usuarios = new List<UsuarioDTO> { new UsuarioDTO { Nome = "User 1" } };
            _mockService.Setup(s => s.ListarAtivosAsync()).ReturnsAsync(usuarios);

            // Act
            var result = await _controller.ListarAtivos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(usuarios, okResult.Value);
        }

        [Fact]
        public async Task BuscarPorCpf_DeveRetornarOk_QuandoEncontrado()
        {
            // Arrange
            var cpf = "123";
            var usuario = new UsuarioDTO { Nome = "User 1", Cpf = cpf };
            _mockService.Setup(s => s.BuscarPorCpfAsync(cpf)).ReturnsAsync(usuario);

            // Act
            var result = await _controller.BuscarPorCpf(cpf);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(usuario, okResult.Value);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarNoContent()
        {
            // Arrange
            var id = 1;
            var dto = new CreateUsuarioDTO { Nome = "Atualizado" };
            _mockService.Setup(s => s.AtualizarAsync(id, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Atualizar(id, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.AtualizarAsync(id, dto), Times.Once);
        }

        [Fact]
        public async Task Desativar_DeveRetornarNoContent()
        {
            // Arrange
            var id = 1;
            _mockService.Setup(s => s.DesativarAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Desativar(id);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.DesativarAsync(id), Times.Once);
        }
    }
}