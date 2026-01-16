using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/estatisticas")]
    public class EstatisticaController : ControllerBase
    {
        private readonly IEstatisticaService _estatisticaService;

        public EstatisticaController(IEstatisticaService estatisticaService)
        {
            _estatisticaService = estatisticaService;
        }

        [HttpGet("top_livros")]
        public async Task<IActionResult> ObterTopLivros()
        {
            var resultado = await _estatisticaService.ObterTopLivrosAsync();
            return Ok(resultado);
        }

        [HttpGet("top_autores")]
        public async Task<IActionResult> ObterTopAutores()
        {
            var resultado = await _estatisticaService.ObterTopAutoresAsync();
            return Ok(resultado);
        }
    }
}