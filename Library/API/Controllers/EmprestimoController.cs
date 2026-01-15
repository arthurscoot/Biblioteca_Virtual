using Library.DTOs;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/emprestimos")]
    public class EmprestimoController : ControllerBase
    {
        private readonly IEmprestimoService _emprestimoService;

        public EmprestimoController(IEmprestimoService emprestimoService)
        {
            _emprestimoService = emprestimoService;
        }

        // 📌 Realizar empréstimo
        [HttpPost]
        public async Task<IActionResult> RealizarEmprestimo([FromBody] CreateEmprestimoDTO dto)
        {
            if (dto == null)
                return BadRequest("Dados inválidos.");

            var resultado = await _emprestimoService.RealizarEmprestimoAsync(dto);

            return CreatedAtAction(nameof(BuscarPorId), new { id = resultado.Id }, resultado);
        }

        // 📌 Devolver empréstimo
        [HttpPut("{emprestimoId}/devolver")]
        public async Task<IActionResult> DevolverEmprestimo(int emprestimoId)
        {
            await _emprestimoService.DevolverEmprestimoAsync(emprestimoId);
            return NoContent();
        }

        // 📌 Renovar empréstimo
        [HttpPut("{emprestimoId}/renovar")]
        public async Task<IActionResult> RenovarEmprestimo(int emprestimoId)
        {
            await _emprestimoService.RenovarEmprestimoAsync(emprestimoId);
            return NoContent();
        }

        // 📌 Listar empréstimos ativos por usuário
        [HttpGet("{usuarioId}/ativos")]
        public async Task<IActionResult> ListarEmprestimosAtivosPorUsuario(int usuarioId)
        {
            var emprestimos = await _emprestimoService
                .ListarEmprestimosAtivosPorUsuarioAsync(usuarioId);

            return Ok(emprestimos);
        }

        // 📌 Histórico de empréstimos por usuário
        [HttpGet("{usuarioId}/historico")]
        public async Task<IActionResult> ListarHistoricoEmprestimosPorUsuario(int usuarioId)
        {
            var emprestimos = await _emprestimoService
                .ListarHistoricoEmprestimosPorUsuarioAsync(usuarioId);

            return Ok(emprestimos);
        }

        // 📌 Buscar empréstimo por ID (interno)
        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            // Corrigido para usar o método correto de busca por ID
            var emprestimo = await _emprestimoService.BuscarPorIdAsync(id);
            return Ok(emprestimo);
        }
    }
}