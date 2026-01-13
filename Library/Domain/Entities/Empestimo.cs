using System;

namespace Library.Entities
{
    public class Emprestimo
    {
        public int Id { get; set; }

        // ===== CHAVES ESTRANGEIRAS =====

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public int LivroId { get; set; }
        public Livro Livro { get; set; } = null!;

        // ===== DATAS =====

        public DateTime DataEmprestimo { get; set; }

        public DateTime DataPrevistaDevolucao { get; set; }

        public DateTime? DataDevolucaoReal { get; set; }

        // ===== MULTA =====

        public decimal ValorMulta { get; set; }

         public decimal ValorMultaPaga { get; set; }

        // ===== CONTROLE =====

        public bool Renovado { get; set; }

        public bool Ativo { get; set; } 
    }
}
