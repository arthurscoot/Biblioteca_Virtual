using Library.DTOs;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Library.Services;

namespace Library.Controllers
{
    [ApiController]
    [Route("api/livros")]
    public class LivroController : ControllerBase
    {
        private readonly ILivroService _livroService;

        // Injeção do service
        public LivroController(ILivroService LivroService)
        {
            _livroService = LivroService;
        }

       
        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] string? titulo, [FromQuery] string? isbn)
        {
            var livros = await _livroService.ListarLivrosAsync(titulo, isbn);
            return Ok(livros);
        }

         [HttpGet("{id}")]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            var livro = await _livroService.BuscarPorIdAsync(id);
            return Ok(livro);
        }
        
        [HttpGet("autor/{id}")]   
         public async Task<IActionResult> BuscarPorAutor(int id)
        {
             var livros = await _livroService.ListarPorAutor(id);
            return Ok(livros);
        }

        [HttpGet("estoque")]
        public async Task<IActionResult> ListarEmEstoque()
        {
            var livros = await _livroService.ListarEmEstoqueAsync();
            return Ok(livros);
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CreateLivroDTO dto)
        {
            var livroCriado = await _livroService.CriarAsync(dto);

            return CreatedAtAction(
                nameof(BuscarPorId),
                new { id = livroCriado.Id },
                livroCriado);
        }

  
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] LivroDTO dto)
        {
            await _livroService.AtualizarAsync(id, dto);
            return NoContent(); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remover(int id)
        {
            await _livroService.RemoverAsync(id);
            return NoContent();
        }

}}