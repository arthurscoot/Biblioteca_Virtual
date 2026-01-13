using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class CreateEmprestimoDTO
    {
        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        public int UsuarioId { get; set; }
        
        [Required(ErrorMessage = "O ID do livro é obrigatório.")]
        public int LivroId { get; set; }
    }
}