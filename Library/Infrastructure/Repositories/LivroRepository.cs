using Library.Data;
using Library.Entities;
using Library.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Library.Data.Repositories
{
    public class LivroRepository : ILivroRepository
    {
        private readonly AppDbContext _context;

        public LivroRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Livro livro)
        {
            await _context.Livro.AddAsync(livro);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Livro livro)
        {
            _context.Livro.Update(livro);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Livro livro)
        {
            _context.Livro.Remove(livro);
            await _context.SaveChangesAsync();
        }

        public async Task<Livro?> BuscarPorIdAsync(int id)
        {
            return await _context.Livro
                .Include(l => l.Autor)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Livro>> ListarAsync(string? titulo, string? isbn)
        {
            var query = _context.Livro.Include(l => l.Autor).AsQueryable();

            if (!string.IsNullOrEmpty(titulo))
                query = query.Where(l => l.Titulo.Contains(titulo));

            if (!string.IsNullOrEmpty(isbn))
                query = query.Where(l => l.ISBN.Contains(isbn));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Livro>> ListarEmEstoqueAsync()
        {
            return await _context.Livro.Include(l => l.Autor).Where(l => l.QuantidadeEstoque > 0).ToListAsync();
        }

        public async Task<IEnumerable<Livro>> ListarPorAutorAsync(int autorId)
        {
            return await _context.Livro.Include(l => l.Autor).Where(l => l.AutorId == autorId).ToListAsync();
        }

        public async Task<bool> ExisteIsbnAsync(string isbn)
        {
            return await _context.Livro.AnyAsync(l => l.ISBN == isbn);
        }

        public async Task<bool> ExisteIsbnEmOutroLivroAsync(int id, string isbn)
        {
            return await _context.Livro.AnyAsync(l => l.Id != id && l.ISBN == isbn);
        }
    }
}