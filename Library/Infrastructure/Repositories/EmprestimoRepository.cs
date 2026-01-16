using Library.Data;
using Library.DTOs;
using Library.Entities;
using Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories
{
    public class EmprestimoRepository : IEmprestimoRepository
    {
        private readonly AppDbContext _context;
        private readonly TimeProvider _timeProvider;

        public EmprestimoRepository(AppDbContext context, TimeProvider timeProvider)
        {
            _context = context;
            _timeProvider = timeProvider;
        }

        public async Task AddAsync(Emprestimo emprestimo)
        {
            await _context.Emprestimos.AddAsync(emprestimo);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Emprestimo emprestimo)
        {
            _context.Emprestimos.Update(emprestimo);
            await _context.SaveChangesAsync();
        }

        public async Task<Emprestimo?> BuscarPorIdAsync(int id)
        {
            return await _context.Emprestimos.FindAsync(id);
        }

        public async Task<IEnumerable<Emprestimo>> ListarAtivosPorUsuarioAsync(int usuarioId)
        {
            return await _context.Emprestimos
                .Where(e => e.UsuarioId == usuarioId && e.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Emprestimo>> ListarHistoricoPorUsuarioAsync(int usuarioId)
        {
            return await _context.Emprestimos
                .Where(e => e.UsuarioId == usuarioId && !e.Ativo)
                .ToListAsync();
        }

        public async Task<bool> PossuiMultaPendenteAsync(int usuarioId)
        {
            return await _context.Emprestimos
                .AnyAsync(e =>
                    e.UsuarioId == usuarioId &&
                    e.ValorMulta - e.ValorMultaPaga > 0 &&
                    e.Ativo == false
                );
        }

        public async Task<bool> ExisteEmprestimoAtivoPorLivroAsync(int livroId)
        {
            return await _context.Emprestimos
                .AnyAsync(e => e.LivroId == livroId && e.Ativo);
        }

        public async Task<IEnumerable<Emprestimo>> ListarTodosAtivosAsync()
        {
            return await _context.Emprestimos
                .Include(e => e.Livro)
                .ThenInclude(l => l.Autor)
                .Where(e => e.Ativo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Emprestimo>> ListarComMultasPendentesAsync()
        {
            return await _context.Emprestimos
                .Where(e => e.ValorMulta > e.ValorMultaPaga)
                .ToListAsync();
        }

        public async Task<IEnumerable<Usuario>> ListarUsuariosComEmprestimosAtrasadosAsync()
        {
            var now = _timeProvider.GetLocalNow().DateTime;
            return await _context.Emprestimos
                .Include(e => e.Usuario)
                .Where(e => e.Ativo && e.DataPrevistaDevolucao < now)
                .Select(e => e.Usuario)
                .Distinct()
                .ToListAsync();
        }
    }
}