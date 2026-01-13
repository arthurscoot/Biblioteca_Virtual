namespace Library.DTOs
{
    public class LivroDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public int AnoPublicacao { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string NomeAutor { get; set; } = string.Empty;
        public int QuantidadeEstoque { get; set; }
        public int AutorId { get; set; }
    }
}
