using System;
using System.Collections.Generic;
using Domain.Exceptions;

namespace Library.Entities
{
    public class Autor
    {
        public int Id { get; private set; }

        public string Nome { get; private set; } = string.Empty;

        public DateTime DataNascimento { get; private set; }

        public string PaisOrigem { get; private set; } = string.Empty;

        public string Biografia { get; private set; } = string.Empty;

        public bool Ativo { get; private set; }

        public ICollection<Livro> Livros { get; private set; } = new List<Livro>();


        protected Autor() { }

        public Autor(string nome, DateTime dataNascimento, string paisOrigem, string biografia)
        {
          Validar(nome, dataNascimento, paisOrigem);

            Nome = nome;
            DataNascimento = dataNascimento;
            PaisOrigem = paisOrigem;
            Biografia = biografia;
            Ativo = true;
        }


        public void Atualizar(string nome, DateTime dataNascimento, string paisOrigem, string biografia)
        {
            if (!Ativo) throw new ValidationException("Não é possível atualizar um autor inativo.");
            
            Validar(nome, dataNascimento, paisOrigem);

            Nome = nome;
            DataNascimento = dataNascimento;
            PaisOrigem = paisOrigem;
            Biografia = biografia;
        }

        public void Desativar()
        {
            if (!Ativo) throw new ValidationException("Autor já está inativo.");
            Ativo = false;
        }

        private void Validar(string nome, DateTime dataNascimento, string paisOrigem)
        {
            if (string.IsNullOrWhiteSpace(nome)) throw new ValidationException("O nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(paisOrigem)) throw new ValidationException("O país de origem é obrigatório.");
            if (dataNascimento.Date > DateTime.Today) throw new ValidationException("A data de nascimento não pode ser futura.");
            
            var hoje = DateTime.Today;
            var idade = hoje.Year - dataNascimento.Year;
            if (dataNascimento.Date > hoje.AddYears(-idade)) idade--;

            if (idade < 16) throw new ValidationException("O autor deve ter no mínimo 16 anos.");
        }
    }
}
