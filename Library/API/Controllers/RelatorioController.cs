using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/relatorios")]
    public class RelatorioController : ControllerBase
    {
        private readonly IRelatorioService _relatorioService;

        public RelatorioController(IRelatorioService relatorioService)
        {
            _relatorioService = relatorioService;
        }

        
        [HttpGet("multas_pendentes")]
        public async Task<IActionResult> ObterTotalMultas()
        {
            var resultado = await _relatorioService.ObterTotalMultasAReceberAsync();
            return Ok(new { Total = resultado });
        }

        [HttpGet("usuarios_atrasados")]
        public async Task<IActionResult> ObterUsuariosAtrasados()
        {
            var resultado = await _relatorioService.ObterUsuariosComEmprestimosAtrasadosAsync();
            return Ok(resultado);
        }
    }
}
