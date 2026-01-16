using System;
using Domain.Exceptions;

namespace Library.Entities
{
    public class Usuario {

        public int Id { get; private set; }

        public string Nome { get; private set; } = string.Empty;

        public string Cpf { get; private set; } = string.Empty;

        public string CpfResponsavel { get; private set; } = string.Empty;

        public string Email { get; private set; } = string.Empty;

        public string Telefone { get; private set; } = string.Empty;

        public DateTime DatadeNascimento { get; private set; }

        public DateTime DatadeCadastro { get; private set; }

        public bool Ativo { get; private set; }

    
        protected Usuario() { }

        public Usuario(string nome, string cpf, string email, string telefone, DateTime dataNascimento, string? cpfResponsavel)
        {
            ValidarRegrasDeCriacao(nome, cpf, email, dataNascimento, cpfResponsavel);

            Nome = nome;
            Cpf = cpf;
            Email = email;
            Telefone = telefone;
            DatadeNascimento = dataNascimento;
            CpfResponsavel = cpfResponsavel ?? string.Empty;
            DatadeCadastro = DateTime.UtcNow;
            Ativo = true;
        }


        public void Atualizar(string nome, string cpf, string email, string telefone)
        {
            if (!Ativo) throw new ValidationException("Não é possível atualizar um usuário inativo.");
            
            Nome = nome;
            Cpf = cpf;
            Email = email;
            Telefone = telefone;
        }

        public void Desativar()
        {
            if (!Ativo) throw new ValidationException("Usuário já está inativo.");
            Ativo = false;
        }

        private void ValidarRegrasDeCriacao(string nome, string cpf, string email, DateTime nascimento, string? cpfResponsavel)
        {
            if (string.IsNullOrWhiteSpace(nome)) throw new ValidationException("Nome é obrigatório.");
            if (string.IsNullOrWhiteSpace(cpf)) throw new ValidationException("CPF é obrigatório.");
            if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("E-mail é obrigatório.");

            var hoje = DateTime.Today;
            var idade = hoje.Year - nascimento.Year;
            if (nascimento.Date > hoje.AddYears(-idade)) idade--;

            if (idade < 16 && string.IsNullOrWhiteSpace(cpfResponsavel))
            {
                throw new ValidationException("O CPF do responsável é obrigatório para usuários menores de 16 anos.");
            }
        }
    }
}