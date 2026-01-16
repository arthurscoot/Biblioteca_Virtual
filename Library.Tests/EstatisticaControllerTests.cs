using Library.Controllers;
using Library.DTOs;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class EstatisticaControllerTests
    {
        private readonly Mock<IEstatisticaService> _mockService;
        private readonly EstatisticaController _controller;

        public EstatisticaControllerTests()
        {
            _mockService = new Mock<IEstatisticaService>();
            _controller = new EstatisticaController(_mockService.Object);
        }

        [Fact]
        public async Task ObterTopLivros_DeveRetornarOk_ComLista()
        {
            // Arrange
            var topLivros = new List<TopLivroDTO> { new TopLivroDTO { Livro = new LivroDTO { Titulo = "Teste" }, QuantidadeEmprestimos = 10 } };
            _mockService.Setup(s => s.ObterTopLivrosAsync()).ReturnsAsync(topLivros);

            // Act
            var result = await _controller.ObterTopLivros();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(topLivros, okResult.Value);
        }

        [Fact]
        public async Task ObterTopAutores_DeveRetornarOk_ComLista()
        {
            // Arrange
            var topAutores = new List<TopAutorDTO> { new TopAutorDTO { Autor = new AutorDto { Nome = "Teste" }, QuantidadeEmprestimos = 5 } };
            _mockService.Setup(s => s.ObterTopAutoresAsync()).ReturnsAsync(topAutores);

            // Act
            var result = await _controller.ObterTopAutores();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(topAutores, okResult.Value);
        }
    }
}