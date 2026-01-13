using System;
using System.Collections.Generic;

namespace Library.Entities
{
    public class Autor
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public DateTime DataNascimento { get; set; }

        public string PaisOrigem { get; set; } = string.Empty;

        public string Biografia { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        // Propriedade de navegação para a coleção de Livros
        public ICollection<Livro> Livros { get; set; } = new List<Livro>();
    }
}
