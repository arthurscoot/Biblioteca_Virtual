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
            var usuarioCriado = await _usuarioService.CriarAsync(dto);

            return CreatedAtAction(
                nameof(BuscarPorCpf),
                new { cpf = usuarioCriado.Cpf },
                usuarioCriado);
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
            return Ok(usuario);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Atualizar(int id, [FromBody] CreateUsuarioDTO dto)
        {
            await _usuarioService.AtualizarAsync(id, dto);
            return NoContent(); 
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Desativar(int id)
        {
            await _usuarioService.DesativarAsync(id);
            return NoContent(); // Sucesso, sem conteúdo para retornar.
        }
    }
}