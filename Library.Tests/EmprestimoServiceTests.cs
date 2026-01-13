using Library.DTOs;
using Library.Entities;
using Library.Interfaces;
using Library.Services;
using Moq;
using Xunit;

namespace Library.Tests
{
    public class EmprestimoServiceTests
    {
        private readonly Mock<IEmprestimoRepository> _mockEmprestimoRepository;
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<ILivroRepository> _mockLivroRepository;
        private readonly Mock<TimeProvider> _mockTimeProvider;
        private readonly EmprestimoService _service;

        public EmprestimoServiceTests()
        {
            _mockEmprestimoRepository = new Mock<IEmprestimoRepository>();
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockLivroRepository = new Mock<ILivroRepository>();
            _mockTimeProvider = new Mock<TimeProvider>();

            // Configura data fixa para testes (01/01/2024)
            var fixedDate = new DateTimeOffset(2024, 1, 1, 12, 0, 0, TimeSpan.Zero);
            _mockTimeProvider.Setup(x => x.GetUtcNow()).Returns(fixedDate);
            _mockTimeProvider.Setup(x => x.LocalTimeZone).Returns(TimeZoneInfo.Utc);

            _service = new EmprestimoService(
                _mockEmprestimoRepository.Object,
                _mockTimeProvider.Object,
                _mockUsuarioRepository.Object,
                _mockLivroRepository.Object
            );
        }

        [Fact]
        public async Task RealizarEmprestimoAsync_DeveCriarEmprestimo_QuandoTudoValido()
        {
            // Arrange
            var dto = new CreateEmprestimoDTO { UsuarioId = 1, LivroId = 1 };
            var usuario = new Usuario { Id = 1, Ativo = true };
            var livro = new Livro { Id = 1, QuantidadeEstoque = 5 };

            _mockUsuarioRepository.Setup(r => r.BuscarPorIdAsync(dto.UsuarioId)).ReturnsAsync(usuario);
            _mockEmprestimoRepository.Setup(r => r.PossuiMultaPendenteAsync(dto.UsuarioId)).ReturnsAsync(false);
            _mockEmprestimoRepository.Setup(r => r.ListarAtivosPorUsuarioAsync(dto.UsuarioId)).ReturnsAsync(new List<Emprestimo>());
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(dto.LivroId)).ReturnsAsync(livro);

            // Act
            var result = await _service.RealizarEmprestimoAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Ativo);
            Assert.Equal(dto.UsuarioId, result.UsuarioId);
            _mockLivroRepository.Verify(r => r.UpdateAsync(livro), Times.Once); // Verifica se decrementou estoque
            _mockEmprestimoRepository.Verify(r => r.AddAsync(It.IsAny<Emprestimo>()), Times.Once);
        }

        [Fact]
        public async Task RealizarEmprestimoAsync_DeveFalhar_QuandoEstoqueZerado()
        {
            // Arrange
            var dto = new CreateEmprestimoDTO { UsuarioId = 1, LivroId = 1 };
            var usuario = new Usuario { Id = 1, Ativo = true };
            var livro = new Livro { Id = 1, QuantidadeEstoque = 0 }; // Sem estoque

            _mockUsuarioRepository.Setup(r => r.BuscarPorIdAsync(dto.UsuarioId)).ReturnsAsync(usuario);
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(dto.LivroId)).ReturnsAsync(livro);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.RealizarEmprestimoAsync(dto));
            Assert.Equal("Livro indisponível no estoque.", ex.Message);
        }

        [Fact]
        public async Task DevolverEmprestimoAsync_DeveDevolver_QuandoEmprestimoAtivo()
        {
            // Arrange
            int emprestimoId = 1;
            var emprestimo = new Emprestimo 
            { 
                Id = emprestimoId, 
                Ativo = true, 
                LivroId = 1, 
                DataPrevistaDevolucao = new DateTime(2024, 1, 10) 
            };
            var livro = new Livro { Id = 1, QuantidadeEstoque = 0 };

            _mockEmprestimoRepository.Setup(r => r.BuscarPorIdAsync(emprestimoId)).ReturnsAsync(emprestimo);
            _mockLivroRepository.Setup(r => r.BuscarPorIdAsync(emprestimo.LivroId)).ReturnsAsync(livro);

            // Act
            var result = await _service.DevolverEmprestimoAsync(emprestimoId);

            // Assert
            Assert.True(result);
            Assert.False(emprestimo.Ativo);
            Assert.NotNull(emprestimo.DataDevolucaoReal);
            _mockLivroRepository.Verify(r => r.UpdateAsync(livro), Times.Once); // Verifica reposição de estoque
        }

        [Fact]
        public async Task RenovarEmprestimoAsync_DeveRenovar_QuandoNaoAtrasado()
        {
            // Arrange
            int emprestimoId = 1;
            var dataPrevistaOriginal = new DateTime(2024, 1, 10); // Futuro em relação ao mock time (01/01/2024)
            var emprestimo = new Emprestimo 
            { 
                Id = emprestimoId, 
                Ativo = true, 
                Renovado = false, 
                DataPrevistaDevolucao = dataPrevistaOriginal 
            };

            _mockEmprestimoRepository.Setup(r => r.BuscarPorIdAsync(emprestimoId)).ReturnsAsync(emprestimo);

            // Act
            var result = await _service.RenovarEmprestimoAsync(emprestimoId);

            // Assert
            Assert.True(result);
            Assert.True(emprestimo.Renovado);
            Assert.True(emprestimo.DataPrevistaDevolucao > dataPrevistaOriginal);
        }

        [Fact]
        public async Task ListarEmprestimosAtivosPorUsuarioAsync_DeveRetornarLista()
        {
            // Arrange
            int usuarioId = 1;
            var lista = new List<Emprestimo> { new Emprestimo { Id = 1, UsuarioId = usuarioId, Ativo = true } };
            _mockEmprestimoRepository.Setup(r => r.ListarAtivosPorUsuarioAsync(usuarioId)).ReturnsAsync(lista);

            // Act
            var result = await _service.ListarEmprestimosAtivosPorUsuarioAsync(usuarioId);

            // Assert
            Assert.Single(result);
        }
    }
}