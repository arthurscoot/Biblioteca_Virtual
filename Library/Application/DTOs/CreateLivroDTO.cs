using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class CreateLivroDTO
    {
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "O título deve ter entre 2 e 150 caracteres.")]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "O ano de publicação é obrigatório.")]
        [Range(1, 9999, ErrorMessage = "Ano de publicação inválido.")]
        public int AnoPublicacao { get; set; }

        [Required(ErrorMessage = "O ISBN é obrigatório.")]
        public string ISBN { get; set; } = string.Empty;

        [Required(ErrorMessage = "A categoria é obrigatória.")]
        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "A quantidade em estoque não pode ser negativa.")]
        public int QuantidadeEstoque { get; set; }

        [Required(ErrorMessage = "O ID do autor é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "ID de autor inválido.")]
        public int AutorId { get; set; }
    }
}
