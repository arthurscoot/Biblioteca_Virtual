using Library.Controllers;
using Library.DTOs;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class AutorControllerTests
    {
        private readonly Mock<IAutorService> _mockService;
        private readonly AutorController _controller;

        public AutorControllerTests()
        {
            _mockService = new Mock<IAutorService>();
            _controller = new AutorController(_mockService.Object);
        }

        [Fact]
        public async Task Listar_DeveRetornarOk_ComListaDeAutores()
        {
            // Arrange
            var autores = new List<AutorDto> { new AutorDto { Id = 1, Nome = "Teste" } };
            _mockService.Setup(s => s.ListarAsync(1, 10)).ReturnsAsync(autores);

            // Act
            var result = await _controller.Listar(1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<AutorDto>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task BuscarPorId_DeveRetornarOk_QuandoEncontrado()
        {
            // Arrange
            var autor = new AutorDto { Id = 1, Nome = "Teste" };
            _mockService.Setup(s => s.BuscarAtivoPorIdAsync(1)).ReturnsAsync(autor);

            // Act
            var result = await _controller.BuscarPorId(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<AutorDto>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task Criar_DeveRetornarCreated_QuandoSucesso()
        {
            // Arrange
            var dto = new CreateAutorDto { Nome = "Novo" };
            var autorCriado = new AutorDto { Id = 1, Nome = "Novo" };
            _mockService.Setup(s => s.CriarAsync(dto)).ReturnsAsync(autorCriado);

            // Act
            var result = await _controller.Criar(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(AutorController.BuscarPorId), createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues?["id"]);
            Assert.Equal(autorCriado, createdResult.Value);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarNoContent()
        {
            // Arrange
            var dto = new CreateAutorDto { Nome = "Atualizado" };
            _mockService.Setup(s => s.AtualizarAsync(1, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Atualizar(1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.AtualizarAsync(1, dto), Times.Once);
        }

        [Fact]
        public async Task Remover_DeveRetornarNoContent()
        {
            // Arrange
            _mockService.Setup(s => s.DesativarAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Remover(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.DesativarAsync(1), Times.Once);
        }
    }
}