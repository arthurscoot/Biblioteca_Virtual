using System;
using Domain.Exceptions;

namespace Library.Entities
{
    public class Livro
    {
        
        public int Id { get; private set; }

        public string Titulo { get; private set; } = string.Empty;

        public string ISBN { get; private set; } = string.Empty;

        public int AnoPublicacao { get; private set; } 

        public int QuantidadeEstoque { get; private set; }

        public string Categoria { get; private set; } = string.Empty;

        public int AutorId { get; private set; }

        public Autor Autor { get; private set; } = null!;
        
        protected Livro() { }

         public Livro(string titulo, string isbn, int anoPublicacao, string categoria, int quantidadeEstoque, int autorId)
        {
            Validar(titulo, isbn, anoPublicacao, categoria, quantidadeEstoque, autorId);

            Titulo = titulo;
            ISBN = isbn;
            AnoPublicacao = anoPublicacao;
            Categoria = categoria;
            QuantidadeEstoque = quantidadeEstoque;
            AutorId = autorId;
        }

        public void Atualizar(string titulo, string isbn, int anoPublicacao, string categoria, int quantidadeEstoque, int autorId)
        {
            Validar(titulo, isbn, anoPublicacao, categoria, quantidadeEstoque, autorId);
            
            Titulo = titulo;
            ISBN = isbn;
            AnoPublicacao = anoPublicacao;
            Categoria = categoria;
            QuantidadeEstoque = quantidadeEstoque;
            AutorId = autorId;
        }

        public void BaixarEstoque()
        {
            if (QuantidadeEstoque <= 0) throw new ValidationException("Livro indisponível no estoque.");
            QuantidadeEstoque--;
        }

        public void ReporEstoque()
        {
            QuantidadeEstoque++;
        }

        public void AssociarAutor(Autor autor)
        {
            Autor = autor;
        }

        private void Validar(string titulo, string isbn, int anoPublicacao, string categoria, int quantidadeEstoque, int autorId)
        {
            if (string.IsNullOrWhiteSpace(titulo)) throw new ValidationException("O título é obrigatório.");
            if (string.IsNullOrWhiteSpace(isbn)) throw new ValidationException("O ISBN é obrigatório.");
            if (isbn.Length < 10 || isbn.Length > 13) throw new ValidationException("O ISBN deve ter entre 10 e 13 caracteres.");
            if (string.IsNullOrWhiteSpace(categoria)) throw new ValidationException("A categoria é obrigatória.");
            if (anoPublicacao <= 0) throw new ValidationException("Ano de publicação inválido.");
            if (quantidadeEstoque < 0) throw new ValidationException("A quantidade em estoque não pode ser negativa.");
            if (autorId <= 0) throw new ValidationException("ID do autor inválido.");
        }
    }
}
