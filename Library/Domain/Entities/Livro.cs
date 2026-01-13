using System;

namespace Library.Entities
{
    public class Livro
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string ISBN { get; set; } = string.Empty;

        public int AnoPublicacao { get; set; } 

        public int QuantidadeEstoque { get; set; }

        public string Categoria { get; set; } = string.Empty;

        // Chave estrangeira para a entidade Autor
        public int AutorId { get; set; }

        // Propriedade de navegação para o Autor
        public Autor Autor { get; set; } = null!;
    }
}
