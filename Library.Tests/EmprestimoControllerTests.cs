using Library.Controllers;
using Library.DTOs;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class EmprestimoControllerTests
    {
        private readonly Mock<IEmprestimoService> _mockService;
        private readonly EmprestimoController _controller;

        public EmprestimoControllerTests()
        {
            _mockService = new Mock<IEmprestimoService>();
            _controller = new EmprestimoController(_mockService.Object);
        }

        [Fact]
        public async Task RealizarEmprestimo_DeveRetornarBadRequest_QuandoDtoNulo()
        {
            // Act
            var result = await _controller.RealizarEmprestimo(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Dados inválidos.", badRequestResult.Value);
        }

        [Fact]
        public async Task RealizarEmprestimo_DeveRetornarCreated_QuandoSucesso()
        {
            // Arrange
            var dto = new CreateEmprestimoDTO { UsuarioId = 1, LivroId = 1 };
            var emprestimoCriado = new EmprestimoDTO { Id = 10, UsuarioId = 1, LivroId = 1 };
            _mockService.Setup(s => s.RealizarEmprestimoAsync(dto)).ReturnsAsync(emprestimoCriado);

            // Act
            var result = await _controller.RealizarEmprestimo(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(EmprestimoController.BuscarPorId), createdResult.ActionName);
            Assert.Equal(10, createdResult.RouteValues?["id"]);
            Assert.Equal(emprestimoCriado, createdResult.Value);
        }

        [Fact]
        public async Task DevolverEmprestimo_DeveRetornarNoContent()
        {
            // Act
            var result = await _controller.DevolverEmprestimo(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.DevolverEmprestimoAsync(1), Times.Once);
        }

        [Fact]
        public async Task RenovarEmprestimo_DeveRetornarNoContent()
        {
            // Act
            var result = await _controller.RenovarEmprestimo(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.RenovarEmprestimoAsync(1), Times.Once);
        }

        [Fact]
        public async Task ListarEmprestimosAtivosPorUsuario_DeveRetornarOk()
        {
            // Arrange
            var lista = new List<EmprestimoDTO> { new EmprestimoDTO { Id = 1 } };
            _mockService.Setup(s => s.ListarEmprestimosAtivosPorUsuarioAsync(1)).ReturnsAsync(lista);

            // Act
            var result = await _controller.ListarEmprestimosAtivosPorUsuario(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(lista, okResult.Value);
        }

        [Fact]
        public async Task ListarHistoricoEmprestimosPorUsuario_DeveRetornarOk()
        {
            // Arrange
            var lista = new List<EmprestimoDTO> { new EmprestimoDTO { Id = 1 } };
            _mockService.Setup(s => s.ListarHistoricoEmprestimosPorUsuarioAsync(1)).ReturnsAsync(lista);

            // Act
            var result = await _controller.ListarHistoricoEmprestimosPorUsuario(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(lista, okResult.Value);
        }

        [Fact]
        public async Task BuscarPorId_DeveRetornarOk()
        {
            // Arrange
            var emprestimo = new EmprestimoDTO { Id = 1 };
            _mockService.Setup(s => s.BuscarPorIdAsync(1)).ReturnsAsync(emprestimo);

            // Act
            var result = await _controller.BuscarPorId(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(emprestimo, okResult.Value);
        }
    }
}