using AutoMapper;
using Library.DTOs;
using Library.Entities;
using Library.Interfaces;
using Library.Services;
using Moq;
using System.Reflection;
using Xunit;

namespace Library.Tests
{
    public class RelatorioServiceTests
    {
        private readonly Mock<IEmprestimoRepository> _mockEmprestimoRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly RelatorioService _service;

        public RelatorioServiceTests()
        {
            _mockEmprestimoRepository = new Mock<IEmprestimoRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new RelatorioService(_mockEmprestimoRepository.Object, _mockMapper.Object);
        }

        // Helper para definir propriedades com set privado (ValorMulta, ValorMultaPaga)
        private void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            prop?.SetValue(obj, value);
        }

        [Fact]
        public async Task ObterTotalMultasAReceberAsync_DeveRetornarZero_QuandoNaoHouverMultas()
        {
            // Arrange
            _mockEmprestimoRepository.Setup(r => r.ListarComMultasPendentesAsync())
                .ReturnsAsync(new List<Emprestimo>());

            // Act
            var result = await _service.ObterTotalMultasAReceberAsync();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task ObterTotalMultasAReceberAsync_DeveRetornarSomaCorreta_QuandoHouverMultasPendentes()
        {
            // Arrange
            var emp1 = new Emprestimo(1, 1, DateTime.Now);
            SetPrivateProperty(emp1, "ValorMulta", 100m);
            SetPrivateProperty(emp1, "ValorMultaPaga", 20m); // Pendente: 80

            var emp2 = new Emprestimo(2, 2, DateTime.Now);
            SetPrivateProperty(emp2, "ValorMulta", 50m);
            SetPrivateProperty(emp2, "ValorMultaPaga", 0m); // Pendente: 50

            var lista = new List<Emprestimo> { emp1, emp2 };

            _mockEmprestimoRepository.Setup(r => r.ListarComMultasPendentesAsync())
                .ReturnsAsync(lista);

            // Act
            var result = await _service.ObterTotalMultasAReceberAsync();

            // Assert
            Assert.Equal(130m, result);
        }

        [Fact]
        public async Task ObterUsuariosComEmprestimosAtrasadosAsync_DeveRetornarListaMapeada()
        {
            // Arrange
            var usuario = new Usuario("Teste", "12345678901", "email@teste.com", "11999999999", DateTime.Now.AddYears(-20), null, DateTime.Now);
            var usuarios = new List<Usuario> { usuario };
            
            var dtos = new List<UsuarioDTO> { new UsuarioDTO { Nome = "Teste", Email = "email@teste.com" } };

            _mockEmprestimoRepository.Setup(r => r.ListarUsuariosComEmprestimosAtrasadosAsync())
                .ReturnsAsync(usuarios);

            _mockMapper.Setup(m => m.Map<IEnumerable<UsuarioDTO>>(usuarios))
                .Returns(dtos);

            // Act
            var result = await _service.ObterUsuariosComEmprestimosAtrasadosAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Teste", result.First().Nome);
            _mockEmprestimoRepository.Verify(r => r.ListarUsuariosComEmprestimosAtrasadosAsync(), Times.Once);
        }
    }
}