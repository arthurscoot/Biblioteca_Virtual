using System;
using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class CreateAutorDto
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3,
            ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        public DateTime DataNascimento { get; set; }

        [Required(ErrorMessage = "O país de origem é obrigatório.")]
        [StringLength(60)]
        public string PaisOrigem { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Biografia { get; set; } = string.Empty;
    }
}
