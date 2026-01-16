using Library.Controllers;
using Library.DTOs;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class RelatorioControllerTests
    {
        private readonly Mock<IRelatorioService> _mockService;
        private readonly RelatorioController _controller;

        public RelatorioControllerTests()
        {
            _mockService = new Mock<IRelatorioService>();
            _controller = new RelatorioController(_mockService.Object);
        }

        [Fact]
        public async Task ObterTotalMultas_DeveRetornarOk_ComObjetoTotal()
        {
            // Arrange
            decimal total = 150.50m;
            _mockService.Setup(s => s.ObterTotalMultasAReceberAsync()).ReturnsAsync(total);

            // Act
            var result = await _controller.ObterTotalMultas();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Como retorna um objeto anônimo, verificamos se não é nulo e se é do tipo esperado
            Assert.NotNull(okResult.Value);
            var prop = okResult.Value.GetType().GetProperty("Total");
            Assert.NotNull(prop);
            Assert.Equal(total, prop.GetValue(okResult.Value));
        }

        [Fact]
        public async Task ObterUsuariosAtrasados_DeveRetornarOk_ComLista()
        {
            // Arrange
            var usuarios = new List<UsuarioDTO> { new UsuarioDTO { Nome = "User" } };
            _mockService.Setup(s => s.ObterUsuariosComEmprestimosAtrasadosAsync()).ReturnsAsync(usuarios);

            // Act
            var result = await _controller.ObterUsuariosAtrasados();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(usuarios, okResult.Value);
        }
    }
}