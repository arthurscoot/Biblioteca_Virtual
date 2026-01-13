using System;

namespace Library.DTOs
{
    public class EmprestimoDTO
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }

        public int LivroId { get; set; }

        public DateTime DataEmprestimo { get; set; }

        public DateTime DataPrevistaDevolucao { get; set; }

        public DateTime? DataDevolucaoReal { get; set; }

        public decimal ValorMulta { get; set; }

        public decimal ValorMultaPaga { get; set; }

        public bool Renovado { get; set; }

        public bool Ativo { get; set; }
    }
}