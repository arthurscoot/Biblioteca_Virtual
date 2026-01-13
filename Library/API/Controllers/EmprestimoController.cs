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
            var sucesso = await _emprestimoService.DevolverEmprestimoAsync(emprestimoId);

            if (!sucesso)
                return NotFound("Empréstimo não encontrado ou já devolvido.");

            return NoContent();
        }

        // 📌 Renovar empréstimo
        [HttpPut("{emprestimoId}/renovar")]
        public async Task<IActionResult> RenovarEmprestimo(int emprestimoId)
        {
            var sucesso = await _emprestimoService.RenovarEmprestimoAsync(emprestimoId);

            if (!sucesso)
                return BadRequest("Empréstimo não pode ser renovado.");

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
            var emprestimo = await _emprestimoService
                .ListarHistoricoEmprestimosPorUsuarioAsync(id);

            if (emprestimo == null)
                return NotFound();

            return Ok(emprestimo);
        }
    }
}