using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class CreateLivroDTO
    {
      
        [StringLength(150, MinimumLength = 2)]
        public string Titulo { get; set; } = string.Empty;

        public int AnoPublicacao { get; set; }

        public string ISBN { get; set; } = string.Empty;

        [StringLength(50)]
        public string Categoria { get; set; } = string.Empty;

        public int QuantidadeEstoque { get; set; }

        public int AutorId { get; set; }
    }
}
