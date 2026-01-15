using Library.DTOs;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutorController : ControllerBase
    {
        private readonly IAutorService _autorService;

        public AutorController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] int page, [FromQuery] int size)
        {
            var autores = await _autorService.ListarAsync(page, size);
            return Ok(autores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var autor = await _autorService.BuscarAtivoPorIdAsync(id);
            return Ok(autor); // se não existir, o service lança NotFoundException
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CreateAutorDto dto)
        {
            var autorCriado = await _autorService.CriarAsync(dto);

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = autorCriado.Id },
                autorCriado
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] CreateAutorDto dto)
        {
            await _autorService.AtualizarAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remover(int id)
        {
            await _autorService.DesativarAsync(id);
            return NoContent();
        }
    }
}
