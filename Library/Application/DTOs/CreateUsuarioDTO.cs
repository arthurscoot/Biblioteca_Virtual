using System.ComponentModel.DataAnnotations;

namespace Library.DTOs
{
    public class CreateUsuarioDTO {

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos, sem pontos ou traços.")]
    public string Cpf { get; set; } = string.Empty;

    [RegularExpression(@"^\d{11}$", ErrorMessage = "O CPF deve conter exatamente 11 dígitos, sem pontos ou traços.")]
    public string? CpfResponsavel { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O formato do e-mail é inválido.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.com$", ErrorMessage = "O e-mail deve ser um endereço .com válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "O telefone é obrigatório.")]
    [RegularExpression(@"^55\s\(\d{2}\)\s\d{4,5}-\d{4}$", ErrorMessage = "O formato do telefone é inválido. Use o formato 55 (XX) XXXXX-XXXX.")]
    public string Telefone { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateTime DataNascimento { get; set; }

    }
}