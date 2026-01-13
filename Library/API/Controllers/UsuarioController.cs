using Library.DTOs;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(UsuarioDTO), 201)]
        [ProducesResponseType(typeof(string), 400)]
        public async Task<IActionResult> Criar([FromBody] CreateUsuarioDTO dto)
        {
            try
            {
                var usuarioCriado = await _usuarioService.CriarAsync(dto);

                // Retorna 201 Created com a localização do novo recurso e o objeto criado.
                return CreatedAtAction(
                    nameof(BuscarPorCpf),
                    new { cpf = usuarioCriado.Cpf },
                    usuarioCriado);
            }
            catch (InvalidOperationException ex)
            {
                // Captura exceções de regra de negócio (ex: CPF duplicado)
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                // Captura outras exceções inesperadas
                return StatusCode(500, "Ocorreu um erro interno ao criar o usuário.");
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UsuarioDTO>), 200)]
        public async Task<IActionResult> ListarAtivos()
        {
            var usuarios = await _usuarioService.ListarAtivosAsync();
            return Ok(usuarios);
        }

        [HttpGet("{cpf}")]
        [ProducesResponseType(typeof(UsuarioDTO), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> BuscarPorCpf(string cpf)
        {
            var usuario = await _usuarioService.BuscarPorCpfAsync(cpf);

            if (usuario == null)
            {
                return NotFound("Usuário não encontrado.");
            }

            return Ok(usuario);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] CreateUsuarioDTO dto)
        {
            try
            {
                var atualizado = await _usuarioService.AtualizarAsync(id, dto);

                if (!atualizado)
                {
                    return NotFound("Usuário não encontrado ou inativo.");
                }

                return NoContent(); // Sucesso, sem conteúdo para retornar.
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Ocorreu um erro interno ao atualizar o usuário.");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Desativar(int id)
        {
            var sucesso = await _usuarioService.DesativarAsync(id);

            if (!sucesso)
            {
                return NotFound("Usuário não encontrado ou já está inativo.");
            }

            return NoContent(); // Sucesso, sem conteúdo para retornar.
        }
    }
}