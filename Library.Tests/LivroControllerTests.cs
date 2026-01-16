using Library.Controllers;
using Library.DTOs;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class LivroControllerTests
    {
        private readonly Mock<ILivroService> _mockService;
        private readonly LivroController _controller;

        public LivroControllerTests()
        {
            _mockService = new Mock<ILivroService>();
            _controller = new LivroController(_mockService.Object);
        }

        [Fact]
        public async Task Listar_DeveRetornarOk_ComListaDeLivros()
        {
            // Arrange
            var livros = new List<LivroDTO> { new LivroDTO { Id = 1, Titulo = "Livro 1" } };
            _mockService.Setup(s => s.ListarLivrosAsync(null, null)).ReturnsAsync(livros);

            // Act
            var result = await _controller.Listar(null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<LivroDTO>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task BuscarPorId_DeveRetornarOk_QuandoEncontrado()
        {
            // Arrange
            var livro = new LivroDTO { Id = 1, Titulo = "Livro 1" };
            _mockService.Setup(s => s.BuscarPorIdAsync(1)).ReturnsAsync(livro);

            // Act
            var result = await _controller.BuscarPorId(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(livro, okResult.Value);
        }

        [Fact]
        public async Task BuscarPorAutor_DeveRetornarOk_ComLista()
        {
            // Arrange
            var livros = new List<LivroDTO> { new LivroDTO { Id = 1, Titulo = "Livro 1" } };
            _mockService.Setup(s => s.ListarPorAutor(1)).ReturnsAsync(livros);

            // Act
            var result = await _controller.BuscarPorAutor(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(livros, okResult.Value);
        }

        [Fact]
        public async Task ListarEmEstoque_DeveRetornarOk_ComLista()
        {
            // Arrange
            var livros = new List<LivroDTO> { new LivroDTO { Id = 1, Titulo = "Livro 1" } };
            _mockService.Setup(s => s.ListarEmEstoqueAsync()).ReturnsAsync(livros);

            // Act
            var result = await _controller.ListarEmEstoque();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(livros, okResult.Value);
        }

        [Fact]
        public async Task Criar_DeveRetornarCreated_QuandoSucesso()
        {
            // Arrange
            var dto = new CreateLivroDTO { Titulo = "Novo Livro" };
            var livroCriado = new LivroDTO { Id = 1, Titulo = "Novo Livro" };
            _mockService.Setup(s => s.CriarAsync(dto)).ReturnsAsync(livroCriado);

            // Act
            var result = await _controller.Criar(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(LivroController.BuscarPorId), createdResult.ActionName);
            Assert.Equal(1, createdResult.RouteValues?["id"]);
            Assert.Equal(livroCriado, createdResult.Value);
        }

        [Fact]
        public async Task Atualizar_DeveRetornarNoContent()
        {
            // Arrange
            var dto = new LivroDTO { Id = 1, Titulo = "Atualizado" };
            _mockService.Setup(s => s.AtualizarAsync(1, dto)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Atualizar(1, dto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Remover_DeveRetornarNoContent()
        {
            // Act
            var result = await _controller.Remover(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.RemoverAsync(1), Times.Once);
        }
    }
}