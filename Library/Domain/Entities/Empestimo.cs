using System;
using Domain.Exceptions;

namespace Library.Entities
{
    public class Emprestimo
    {
        public int Id { get; private set; }

        public int UsuarioId { get; private set; }
        public Usuario Usuario { get; private set; } = null!;

        public int LivroId { get; private set; }
        public Livro Livro { get; private set; } = null!;

        public DateTime DataEmprestimo { get; private set; }

        public DateTime DataPrevistaDevolucao { get; private set; }

        public DateTime? DataDevolucaoReal { get; private set; }

        public decimal ValorMulta { get; private set; }

        public decimal ValorMultaPaga { get; private set; }

        public bool Renovado { get; private set; }

        public bool Ativo { get; private set; } 

        
        protected Emprestimo() { }

        public Emprestimo(int usuarioId, int livroId)
        {
            UsuarioId = usuarioId;
            LivroId = livroId;
            DataEmprestimo = DateTime.Now; 
            DataPrevistaDevolucao = DataEmprestimo.AddDays(14);
            Ativo = true;
            Renovado = false;
        }

        public void Renovar()
        {
            if (!Ativo) throw new ValidationException("Empréstimo já finalizado não pode ser renovado.");
            if (Renovado) throw new ValidationException("Empréstimo já foi renovado uma vez.");
            if (DateTime.Now > DataPrevistaDevolucao) throw new ValidationException("Não é possível renovar um empréstimo em atraso.");

            DataPrevistaDevolucao = DataPrevistaDevolucao.AddDays(14);
            Renovado = true;
        }

        public void Devolver()
        {
            if (!Ativo) throw new ValidationException("Empréstimo já foi devolvido.");

            DataDevolucaoReal = DateTime.Now;
            Ativo = false;
            CalcularMulta();
        }

        private void CalcularMulta()
        {
            if (DataDevolucaoReal.HasValue && DataDevolucaoReal.Value.Date > DataPrevistaDevolucao.Date)
            {
                int diasAtraso = (DataDevolucaoReal.Value.Date - DataPrevistaDevolucao.Date).Days;
                ValorMulta = diasAtraso * 2.0m;
            }
        }
    }
}
