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

        // Injeção do service
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
            var autor = await _autorService.BuscarPorIdAsync(id);

            if (autor == null)
                return NotFound("Autor não encontrado.");

            return Ok(autor);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CreateAutorDto dto)
        {
            try
            {
                var autorCriado = await _autorService.CriarAsync(dto);

                // Retorna 201 + localização do recurso
                return CreatedAtAction(
                    nameof(BuscarPorId),
                    new { id = autorCriado.Id },
                    autorCriado
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] CreateAutorDto dto)
        {
            try
            {
                var atualizado = await _autorService.AtualizarAsync(id, dto);

                if (!atualizado)
                    return NotFound("Autor não encontrado.");

                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remover(int id)
        {
            var sucesso = await _autorService.DesativarAsync(id);

            if (!sucesso) 
                return NotFound("Autor não encontrado ou já está inativo.");
        
            return NoContent(); 
        }

}}
